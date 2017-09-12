using LearnMVC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class QuizOverviewVM
    {
        public Category[] Categories { get; set; }

        public int NumberOfQuestions { get; set; }

        public int NumberOfQuizUnits { get; set; }

        public int NumberOfCategories { get; set; }
    }
}
