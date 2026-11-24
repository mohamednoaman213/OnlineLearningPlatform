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
  public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Title).IsRequired().HasMaxLength(200);
            b.Property(c => c.Description).HasMaxLength(2000);
            b.Property(c => c.Price).HasPrecision(10, 2);
            b.Property(c => c.Level).HasConversion<string>().HasMaxLength(20);
            b.Property(c => c.IsPublished).HasDefaultValue(false);

            b.HasOne(c => c.Category)
             .WithMany(cat => cat.Courses)
             .HasForeignKey(c => c.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
      
        }
    
    }
}
