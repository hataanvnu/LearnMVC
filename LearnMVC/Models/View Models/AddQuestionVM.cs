﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMVC.Models.View_Models
{
    public class AddQuestionVM
    {
        public string QuestionText { get; set; }

        public int Order { get; set; }

        public SelectListItem[] PossibleQuizUnits { get; set; }

        public int SelectedQuizUnitId { get; set; }
    }
}
