using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class MembersIndexVM
    {
        public string UserName { get; set; }

        public string CategoryName { get; set; }

        public string QuizUnitName { get; set; }

        public string QuestionNumber { get; set; }

        public bool HasPreviousProgress { get; set; }
    }
}
