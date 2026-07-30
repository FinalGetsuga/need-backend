using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.HasOne(w => w.Business)
            .WithOne(b => b.WorkSchedule)
            .HasForeignKey<WorkSchedule>(w => w.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(w => w.BusinessId)
            .IsUnique();
    }
}