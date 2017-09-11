using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class QuizUnitSubVM
    {
        public string QuizUnitHeader { get; set; }

        public string QuizUnitContent { get; set; }

        public List<QuestionSubVM> Questions { get; set; }

        public int Order { get; set; }

    }
}
