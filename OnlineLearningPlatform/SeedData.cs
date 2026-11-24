
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.models;

namespace OnlineLearningPlatform;

public static class SeedData
{
    public static void Seed(this ModelBuilder mb)
    {
        mb.Entity<Instructor>().HasData(
            new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed@learn.com", Bio = "Backend engineer", JoinedAt = new DateTime(2023, 1, 10) },
            new Instructor { Id = 2, FullName = "Sara Ali", Email = "sara@learn.com", JoinedAt = new DateTime(2023, 6, 1) });

        mb.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming", Description = "All programming topics" },
            new Category { Id = 2, Name = "Backend", ParentCategoryId = 1 },
            new Category { Id = 3, Name = "ASP.NET", ParentCategoryId = 2 },
            new Category { Id = 4, Name = "Design" });

        mb.Entity<Course>().HasData(
            new Course { Id = 1, Title = "ASP.NET Core Basics", Description = "Intro to ASP.NET Core", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedAt = new DateTime(2024, 1, 5), InstructorId = 1, CategoryId = 3 },
            new Course { Id = 2, Title = "Advanced EF Core", Price = 99.00m, Level = CourseLevel.Advanced, IsPublished = true, PublishedAt = new DateTime(2024, 3, 12), InstructorId = 1, CategoryId = 2 },
            new Course { Id = 3, Title = "UI Design 101", Price = 29.50m, Level = CourseLevel.Beginner, IsPublished = false, InstructorId = 2, CategoryId = 4 });

        mb.Entity<Student>().HasData(
            new Student { Id = 1, FullName = "Omar Khaled", Email = "omar@mail.com", DateOfBirth = new DateTime(2000, 5, 20) },
            new Student { Id = 2, FullName = "Mona Samir", Email = "mona@mail.com", DateOfBirth = new DateTime(1999, 11, 2) });

        mb.Entity<Enrollment>().HasData(
            new Enrollment { StudentId = 1, CourseId = 1, EnrolledAt = new DateTime(2024, 2, 1), ProgressPercentage = 40, Status = EnrollmentStatus.Active },
            new Enrollment { StudentId = 2, CourseId = 1, EnrolledAt = new DateTime(2024, 2, 3), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.5m },
            new Enrollment { StudentId = 1, CourseId = 2, EnrolledAt = new DateTime(2024, 4, 1), ProgressPercentage = 10, Status = EnrollmentStatus.Active });
    }
}