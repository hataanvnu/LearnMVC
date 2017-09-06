using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LearnMVC.Models.View_Models;
using System.Linq;
using System.Collections.Generic;

namespace LearnMVC.Models.Entities
{
    public partial class QuizDbContext : DbContext
    {


        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
        {
        }


        public MembersIndexVM GetMembersIndexVMById(string id, string userName)
        {
            MembersIndexVM membersIndexVM;

            var latestProgress = Progress
                .Include(p => p.Question)
                .Include(p => p.Question.QuizUnit)
                .Include(p => p.Question.QuizUnit.Category)
                .FirstOrDefault(m => m.MemberId == id);

            if (latestProgress == null)
            {
                // Användaren har inte klarat av någon fråga än:
                membersIndexVM = new MembersIndexVM
                {
                    HasPreviousProgress = false,
                    UserName = userName,
                };
            }
            else
            {
                // Läs in tidigare progress
                string categoryTitle = latestProgress.Question.QuizUnit.Category.Title;
                string quizUnitTitle = latestProgress.Question.QuizUnit.InfoTextHeader;
                string questionTitle = latestProgress.Question.QuestionText;

                membersIndexVM = new MembersIndexVM
                {
                    HasPreviousProgress = true,
                    CategoryName = categoryTitle,
                    QuestionNumber = questionTitle,
                    QuizUnitName = quizUnitTitle,
                    UserName = userName,
                };
            }

            return membersIndexVM;
        }

        internal SidebarVM[] GetSidebarVMList(string memberID)
        {
            var q = Category
                .OrderBy(c => c.Order)
                .Select(c => new SidebarVM
            {
                CategoryID = c.CategoryId,
                CategoryName = c.Title,
                IsDone = false, // todo - lägg till logik för detta.
            }).ToArray();

            return q;
        }
    }
}
