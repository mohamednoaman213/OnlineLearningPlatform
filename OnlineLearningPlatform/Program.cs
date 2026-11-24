
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.models;
using OnlineLearningPlatform.Configurations;
namespace OnlineLearningPlatform;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("===== CREATE =====");
        int instructorId = AddInstructor("Mostafa Adel", "mostafa@learn.com", "Cloud trainer");
        int courseId = AddCourse(instructorId, 1, "Docker for .NET", 59m, CourseLevel.Intermediate);
        AddCourse(999, 1, "Ghost Course", 10m, CourseLevel.Beginner);        // invalid instructor
        AddStudents();
        EnrollStudent(1, courseId);   // refused: course not published yet
        EnrollStudent(1, 1);          // refused: already enrolled
        EnrollStudent(999, 1);        // refused: student missing
        EnrollStudent(2, 2);          // OK

        Console.WriteLine("\n===== READ =====");
        FindCourse(1, "Advanced EF Core");
        FindCourse(999, "Nope");
        FilterCourses(30m, 100m);
        PrintSubCategories(1);
        PrintTopLevelCategories();
        CourseRoster(1);

        Console.WriteLine("\n===== UPDATE =====");
        ChangePrice(1, 54.99m);
        UpdateProgress(1, 2, 100);
        UpdateProgress(1, 2, 150);    // rejected
        PublishCourse(courseId);
        EnrollStudent(1, courseId);   // now accepted

        Console.WriteLine("\n===== DELETE =====");
        Unenroll(1, courseId);
        DeleteCategoryUnsafe(1);
        DeleteCategorySafe(1);
        DeleteCategorySafe(4);        // no sub-categories -> deleted
        DeleteCourse(1);
    }

    // ---------------------------------------------------------------- CREATE
    static int AddInstructor(string name, string email, string? bio)
    {
        using var db = new AppDbContext();
        var instructor = new Instructor { FullName = name, Email = email, Bio = bio, JoinedAt = DateTime.UtcNow };
        db.Instructors.Add(instructor);
        db.SaveChanges();
        Console.WriteLine($"Instructor added. Id = {instructor.Id}");
        return instructor.Id;
    }

    static int AddCourse(int instructorId, int categoryId, string title, decimal price, CourseLevel level)
    {
        using var db = new AppDbContext();
        if (!db.Instructors.Any(i => i.Id == instructorId))
        {
            Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist.");
            return 0;
        }
        if (!db.Categories.Any(c => c.Id == categoryId))
        {
            Console.WriteLine($"Cannot add course: category {categoryId} does not exist.");
            return 0;
        }
        if (price < 0)
        {
            Console.WriteLine("Cannot add course: price can't be negative.");
            return 0;
        }

        var course = new Course { Title = title, Price = price, Level = level, InstructorId = instructorId, CategoryId = categoryId };
        db.Courses.Add(course);
        db.SaveChanges();
        Console.WriteLine($"Course added. Id = {course.Id}");
        return course.Id;
    }

    static void AddStudents()
    {
        using var db = new AppDbContext();
        var suffix = Guid.NewGuid().ToString("N")[..6];   // keeps emails unique across runs
        var students = new List<Student>
        {
            new() { FullName = "Youssef Nabil", Email = $"youssef{suffix}@mail.com", DateOfBirth = new DateTime(2001, 3, 15) },
            new() { FullName = "Hana Tarek",    Email = $"hana{suffix}@mail.com",    DateOfBirth = new DateTime(2002, 7, 9) },
            new() { FullName = "Karim Fathy",   Email = $"karim{suffix}@mail.com",   DateOfBirth = new DateTime(1998, 12, 30) },
        };
        db.Students.AddRange(students);
        db.SaveChanges();
        Console.WriteLine("Students added. Ids = " + string.Join(", ", students.Select(s => s.Id)));
    }

    static void EnrollStudent(int studentId, int courseId)
    {
        using var db = new AppDbContext();
        var student = db.Students.Find(studentId);
        if (student is null) { Console.WriteLine($"Enrollment refused: student {studentId} not found."); return; }

        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Enrollment refused: course {courseId} not found."); return; }

        if (!course.IsPublished) { Console.WriteLine($"Enrollment refused: '{course.Title}' is not published."); return; }

        if (db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        {
            Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
            return;
        }

        db.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            ProgressPercentage = 0,
            Status = EnrollmentStatus.Active
        });
        db.SaveChanges();
        Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
    }

    // ------------------------------------------------------------------ READ
    static void FindCourse(int id, string title)
    {
        using var db = new AppDbContext();
        var byId = db.Courses.Find(id);
        Console.WriteLine(byId is null ? $"Course with Id {id}: not found" : $"Find({id}) -> {byId.Title}");

        var byTitle = db.Courses.FirstOrDefault(c => c.Title == title);
        Console.WriteLine(byTitle is null ? $"Course titled '{title}': not found" : $"FirstOrDefault('{title}') -> Id {byTitle.Id}");
    }

    static void FilterCourses(decimal min, decimal max)
    {
        using var db = new AppDbContext();
        var courses = db.Courses
            .Where(c => c.Price >= min && c.Price <= max)
            .OrderByDescending(c => c.Price)
            .Select(c => new { c.Title, c.Level, c.Price })
            .ToList();

        Console.WriteLine($"Courses priced {min} - {max}:");
        foreach (var c in courses) Console.WriteLine($"  {c.Title} | {c.Level} | {c.Price:C}");
    }

    static void PrintSubCategories(int parentId)
    {
        using var db = new AppDbContext();
        var subs = db.Categories.Where(c => c.ParentCategoryId == parentId).ToList();
        Console.WriteLine($"Sub-categories of {parentId}:");
        if (subs.Count == 0) Console.WriteLine("  (none)");
        foreach (var s in subs) Console.WriteLine($"  {s.Id}: {s.Name}");
    }

    static void PrintTopLevelCategories()
    {
        using var db = new AppDbContext();
        Console.WriteLine("Top-level categories:");
        foreach (var c in db.Categories.Where(c => c.ParentCategoryId == null))
            Console.WriteLine($"  {c.Id}: {c.Name}");
    }

    static void CourseRoster(int courseId)
    {
        using var db = new AppDbContext();
        var studentIds = db.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToList();

        Console.WriteLine($"Course {courseId} has {studentIds.Count} student(s):");
        var names = db.Students
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => s.FullName)
            .ToList();
        foreach (var n in names) Console.WriteLine($"  {n}");
    }

    // ---------------------------------------------------------------- UPDATE
    static void ChangePrice(int courseId, decimal newPrice)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Course {courseId} not found."); return; }
        if (newPrice < 0) { Console.WriteLine("Price can't be negative."); return; }

        var old = course.Price;
        course.Price = newPrice;
        db.SaveChanges();
        Console.WriteLine($"Price of '{course.Title}' changed: {old} -> {newPrice}");
    }

    static void UpdateProgress(int studentId, int courseId, int progress)
    {
        if (progress < 0 || progress > 100)
        {
            Console.WriteLine($"Rejected: progress {progress} must be between 0 and 100.");
            return;
        }

        using var db = new AppDbContext();
        var enrollment = db.Enrollments.Find(studentId, courseId);   // key order: StudentId, CourseId
        if (enrollment is null) { Console.WriteLine("Enrollment not found."); return; }

        enrollment.ProgressPercentage = progress;
        if (progress == 100) enrollment.Status = EnrollmentStatus.Completed;
        else if (enrollment.Status == EnrollmentStatus.Completed) enrollment.Status = EnrollmentStatus.Active;

        db.SaveChanges();
        Console.WriteLine($"Progress updated to {progress}% (status: {enrollment.Status}).");
    }

    static void PublishCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Course {courseId} not found."); return; }
        if (course.IsPublished) { Console.WriteLine($"'{course.Title}' is already published."); return; }

        course.IsPublished = true;
        course.PublishedAt = DateTime.UtcNow;
        db.SaveChanges();
        Console.WriteLine($"'{course.Title}' published at {course.PublishedAt:u}.");
    }

    // ---------------------------------------------------------------- DELETE
    static void Unenroll(int studentId, int courseId)
    {
        using var db = new AppDbContext();
        var enrollment = db.Enrollments.Find(studentId, courseId);
        if (enrollment is null) { Console.WriteLine("Enrollment not found."); return; }

        db.Enrollments.Remove(enrollment);
        db.SaveChanges();
        Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");
    }

    static void DeleteCategoryUnsafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category is null) { Console.WriteLine("Category not found."); return; }

        try
        {
            db.Categories.Remove(category);
            db.SaveChanges();
            Console.WriteLine("Category deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Unsafe delete failed: " + (ex.InnerException?.Message ?? ex.Message));
        }
    }

    static void DeleteCategorySafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category is null) { Console.WriteLine("Category not found."); return; }

        if (db.Categories.Any(c => c.ParentCategoryId == categoryId))
        {
            Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
            return;
        }
        if (db.Courses.Any(c => c.CategoryId == categoryId))
        {
            Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
            return;
        }

        db.Categories.Remove(category);
        db.SaveChanges();
        Console.WriteLine($"Category '{category.Name}' deleted.");
    }

    static void DeleteCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine("Course not found."); return; }

        int before = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments before delete: {before}");

        db.Courses.Remove(course);
        db.SaveChanges();

        // The enrollments disappeared because the Course -> Enrollment relationship uses
        // DeleteBehavior.Cascade (the default for a required FK). SQL Server applies
        // ON DELETE CASCADE, so deleting the course deletes its enrollment rows too.
        int after = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments after delete: {after}");
    }
}
