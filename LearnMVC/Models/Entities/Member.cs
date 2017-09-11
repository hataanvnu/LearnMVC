using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class Member
    {
        public string MemberId { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
