namespace OnlineLearningPlatform.models
{

    public class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public int ProgressPercentage { get; set; }
        public EnrollmentStatus Status { get; set; }
        public decimal? FinalGrade { get; set; }   // only when Completed

        public Student Student { get; set; } 
        public Course Course { get; set; } 
    }
}
