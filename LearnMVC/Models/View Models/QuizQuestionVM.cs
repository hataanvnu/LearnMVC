using LearnMVC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class QuizQuestionVM
    {
        public string QuestionText { get; set; }

        public Answer[] Answers { get; set; }

        public int QuestionId { get; set; }

    }
}
