using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.models
{
public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Bio { get; set; } //optional
        public string Email { get; set; }
        public DateTime JoinedAt { get; set; }

        // navigation
        public ICollection<Course>Courses { get; set; }=new List<Course>();
    
    }
}
