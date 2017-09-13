using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class EditQuestionVM
    {
        [Required]
        [Display(Name = "Question text")]
        public string QuestionText { get; set; }

        public int Order { get; set; }

        public SelectListItem[] PossibleQuizUnits { get; set; }

        [Required]
        [Display(Name = "Quit unit")]
        public int SelectedQuizUnitId { get; set; }

        public string[] Answers { get; set; }
    }
}
