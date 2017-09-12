using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class EditCategoryVM
    {
        [Required]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters long")]
        [Display(Name = "Category title")]
        public string CategoryTitle { get; set; }

        public int Order { get; set; }
    }
}
