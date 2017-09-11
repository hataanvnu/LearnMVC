using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class AddQuizUnitVM
    {
        [Required]
        [Display(Name = "Quiz unit header")]
        public string QuizUnitHeader { get; set; }

        [Required]
        [Display(Name = "Quiz unit content")]
        public string QuizUnitContent { get; set; }

        public int Order { get; set; }

        public SelectListItem[] Categories { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int SelectedCategoryId { get; set; }
    }
}
