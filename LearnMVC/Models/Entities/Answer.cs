using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public int? QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
