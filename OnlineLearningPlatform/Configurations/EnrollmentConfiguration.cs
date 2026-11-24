using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> b)
        {
            // Composite key => a student can enroll in the same course only once
            b.HasKey(e => new { e.StudentId, e.CourseId });

            b.Property(e => e.EnrolledAt).IsRequired();
            b.Property(e => e.ProgressPercentage).HasDefaultValue(0);
            b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            b.Property(e => e.FinalGrade).HasPrecision(5, 2);

            b.ToTable(t => t.HasCheckConstraint("CK_Enrollment_Progress",
                "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100"));

            b.HasOne(e => e.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(e => e.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            // Cascade: deleting a course deletes its enrollments
            b.HasOne(e => e.Course)
             .WithMany(c => c.Enrollments)
             .HasForeignKey(e => e.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    
    }
}
