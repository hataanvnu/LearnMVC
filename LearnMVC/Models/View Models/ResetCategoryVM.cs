using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class ResetCategoryVM
    {
        public int CategoryId { get; set; }

        public SidebarVM[] SidebarList { get; set; }
    }
}
