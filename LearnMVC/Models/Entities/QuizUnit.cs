using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class QuizUnit
    {
        public QuizUnit()
        {
            Question = new HashSet<Question>();
        }

        public int QuizUnitId { get; set; }
        public string InfoTextHeader { get; set; }
        public string InfoTextContent { get; set; }
        public int Order { get; set; }
        public int? CategoryId { get; set; }

        public Category Category { get; set; }
        public ICollection<Question> Question { get; set; }
    }
}
