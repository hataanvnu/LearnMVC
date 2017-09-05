using System;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class Progress
    {
        public int ProgressId { get; set; }
        public string MemberId { get; set; }
        public int? QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
