using System.Text;
using Domain.Identity;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Scalar.AspNetCore;
using Service.Implementation;
using Service.Interface;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Firebase Admin SDK
        var firebaseJson = builder.Configuration["Firebase:ServiceAccountJson"];
        
        GoogleCredential credential;
        if (!string.IsNullOrEmpty(firebaseJson))
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(firebaseJson));
            credential = GoogleCredential.FromServiceAccountCredential(
                Google.Apis.Auth.OAuth2.ServiceAccountCredential
                    .FromServiceAccountData(stream));
        }
        else
        {
            credential = GoogleCredential.GetApplicationDefault();
        }

        FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        });
        
        // Firestore
        FirestoreDbBuilder firestoreBuilder = new FirestoreDbBuilder
        {
            ProjectId = builder.Configuration["Firebase:ProjectId"],
            Credential = credential
        };

        var firestoreDb = firestoreBuilder.Build();
        builder.Services.AddSingleton(firestoreDb);
        
        // Frontend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        // Database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // Identity
        builder.Services.AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        // JWT Auth via Firebase
        var projectId = builder.Configuration["Firebase:ProjectId"];

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var uid = context.Principal?.FindFirst("user_id")?.Value;
                        if (string.IsNullOrEmpty(uid))
                        {
                            context.Fail("Missing user_id claim");
                            return;
                        }
                        
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<AppUser>>();

                        var user = await userManager.FindByLoginAsync("Firebase", uid);

                        if (user == null)
                        {
                            var email = context.Principal?.FindFirst(
                                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                            )?.Value ?? context.Principal?.FindFirst("email")?.Value;

                            user = new AppUser
                            {
                                UserName = uid,
                                Email = email,
                                FirebaseUserId = uid,
                                DisplayName = context.Principal?.FindFirst("name")?.Value ?? string.Empty
                            };

                            await userManager.CreateAsync(user);
                            await userManager.AddLoginAsync(user,
                                new UserLoginInfo("Firebase", uid, "Firebase"));
                        }
                    }
                };
            });
        
        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new();
                document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter your Firebase JWT token"
                    }
                };
                return Task.CompletedTask;
            });
        });
        
        // Repository
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Services
        builder.Services.AddScoped<IBusinessService, BusinessService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        
        app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }));

        app.UseCors("FrontendPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}