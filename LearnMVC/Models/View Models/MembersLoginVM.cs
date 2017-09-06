using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class MembersLoginVM
    {
        [Required(ErrorMessage = "You must provide a user name.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "You must provide a password.")]
        public string Password { get; set; }
    }
}
