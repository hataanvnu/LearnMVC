﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class AddAnswerVM
    {
        public string AnswerText { get; set; }

        public bool IsCorrect { get; set; }
    }
}
