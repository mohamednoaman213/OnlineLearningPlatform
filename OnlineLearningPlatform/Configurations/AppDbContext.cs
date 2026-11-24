
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.models;

namespace OnlineLearningPlatform;

public class AppDbContext : DbContext
{ 
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=LearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

     
       modelBuilder.Seed();
    }
}