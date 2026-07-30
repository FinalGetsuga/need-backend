using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.HasOne(b => b.Term)
            .WithOne(t => t.Booking)
            .HasForeignKey<Booking>(b => b.TermId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.TermId)
            .IsUnique();

        builder.HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.CustomerId);
    }
}