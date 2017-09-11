using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class QuestionSubVM
    {
        public string QuestionText { get; set; }

        public List<AnswerSubVM> Answers { get; set; }

        public int Order { get; set; }

    }
}
