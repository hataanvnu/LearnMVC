using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class Question
    {
        public Question()
        {
            Answer = new HashSet<Answer>();
            Progress = new HashSet<Progress>();
        }

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int? QuizUnitId { get; set; }

        public QuizUnit QuizUnit { get; set; }
        public ICollection<Answer> Answer { get; set; }
        public ICollection<Progress> Progress { get; set; }
    }
}
