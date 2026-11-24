using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.models;

namespace OnlineLearningPlatform.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> b)
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.FullName).IsRequired().HasMaxLength(100);
            b.Property(i => i.Email).IsRequired().HasMaxLength(150);
            b.HasIndex(i => i.Email).IsUnique();
            b.Property(i => i.Bio).HasMaxLength(500);
            b.Property(i => i.JoinedAt).IsRequired();

            b.HasMany(i => i.Courses)
             .WithOne(c => c.Instructor)
             .HasForeignKey(c => c.InstructorId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    

    }
}
