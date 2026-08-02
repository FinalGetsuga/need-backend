using System.Security.Claims;
using System.Text;
using Domain.Identity;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
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
using Web.Auth;
using Web.Mappers;

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
        
        // Hangfire
        builder.Services.AddHangfire(config => config
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
        builder.Services.AddHangfireServer();
        
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
                    RoleClaimType = ClaimTypes.Role,
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
                                FirstName = email,
                                LastName = email,
                                FirebaseUserId = uid,
                                DisplayName = context.Principal?.FindFirst("name")?.Value ?? string.Empty
                            };

                            await userManager.CreateAsync(user);
                            await userManager.AddLoginAsync(user,
                                new UserLoginInfo("Firebase", uid, "Firebase"));
                        }

                        var roles = await userManager.GetRolesAsync(user);
                        var identity = (ClaimsIdentity)context.Principal.Identity!;
                        
                        identity.AddClaim(new Claim(AppClaimTypes.InternalUserId, user.Id));
                        
                        foreach (var role in roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
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
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IBusinessService, BusinessService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ITermGenerationService, TermGenerationService>();
        builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
        
        // Helper
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    
        // Mappers
        builder.Services.AddScoped<BookingMapper>();
        builder.Services.AddScoped<BusinessMapper>();
        builder.Services.AddScoped<CategoryMapper>();
        builder.Services.AddScoped<EmployeeMapper>();
        builder.Services.AddScoped<ReviewMapper>();
        builder.Services.AddScoped<WorkScheduleMapper>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseHangfireDashboard();
        }

        app.UseHttpsRedirection();
        
        app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { error = message });
        }));

        app.UseCors("FrontendPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        RecurringJob.AddOrUpdate<IBookingService>(
            "complete-past-bookings",
            x => x.MarkPastBookingsCompletedAsync(),
            "5 0 * * *");
        
        RecurringJob.AddOrUpdate<ITermGenerationService>(
            "generate-terms",
            x => x.GenerateForAllBusinessesAsync(),
            "15 0 * * *");
        
        app.Run();
    }
}