using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class WorkingDayConfiguration : IEntityTypeConfiguration<WorkingDay>
{
    public void Configure(EntityTypeBuilder<WorkingDay> builder)
    {
        builder.HasOne(w => w.WorkSchedule)
            .WithMany(d => d.WorkingDays)
            .HasForeignKey(w => w.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.WorkScheduleId, w.DayOfWeek })
            .IsUnique();
    }
}