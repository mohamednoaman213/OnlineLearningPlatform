namespace OnlineLearningPlatform.models
{
  
   public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }//optional
        public decimal Price { get; set; }
        public CourseLevel Level { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
     


        //navigation
        public int InstructorId {  get; set; }
        public Instructor Instructor { get; set; }
         
        public Category Category { get; set; }
        public int CategoryId { get; set; }


        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    }
}
