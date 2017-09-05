using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class Category
    {
        public Category()
        {
            QuizUnit = new HashSet<QuizUnit>();
        }

        public int CategoryId { get; set; }
        public string Title { get; set; }

        public ICollection<QuizUnit> QuizUnit { get; set; }
    }
}
