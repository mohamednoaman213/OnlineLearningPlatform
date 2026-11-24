using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.models
{
public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }//optional
        public int ParentCategoryId { get; set; }
        //navigation
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; }=new List<Category>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
