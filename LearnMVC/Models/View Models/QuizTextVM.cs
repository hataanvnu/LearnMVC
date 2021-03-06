﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class QuizTextVM
    {
        public string CategoryName { get; set; }

        public string TextHeader { get; set; }

        public string TextContent { get; set; }

        public SidebarVM[] SidebarArray { get; set; }

        public int QuizUnitId { get; set; }

        public bool FinishedACategory { get; set; }

        public int CategoryId { get; set; }

        public double CategoryProgress { get; set; }
    }
}
