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

        /// <summary>
        /// </summary>
        /// <param name="id">The member ID of the current user</param>
        /// <returns>The next question of the Quiz unit. If there are no more questions in this quiz unit, it returns NULL.</returns>
        private Question GetNextQuestionByMemberId(string id)
        {
            // hämta användarens senaste Progress för att få fram QuestionId
            var latestDate = Progress
                .Where(p => p.MemberId == id)
                .Max(p => p.DateCreated);

            var latestQuestion = Progress
                .Where(p => p.MemberId == id)
                .SingleOrDefault(p => p.DateCreated.ToString() == latestDate.ToString())
                .Question;

            // hämta Det quiz-unitet
            var latestQuizUnit = latestQuestion.QuizUnit;

            // Ta fram nästa fråga i QuizUnitet
            var currentQuestionOrder = latestQuestion.Order;
            var que = Question
                .Where(q => q.QuizUnit.QuizUnitId == latestQuizUnit.QuizUnitId)
                .Where(q => q.Order > latestQuestion.Order)
                .OrderBy(q => q.Order)
                .FirstOrDefault();

            // returnerar frågan om någon hittades, annars returneras null.
            if (que != null)
            {
                return que;
            }
            else
            {
                return null;
            }
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
                int questionId = latestProgress.Question.QuestionId;

                membersIndexVM = new MembersIndexVM
                {
                    HasPreviousProgress = true,
                    CategoryName = categoryTitle,
                    QuestionText = questionTitle,
                    QuestionID = questionId,
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

        public QuizTextVM GetQuizTextVMById(int id)
        {
            // todo - lägg till logik för att välja rätt quizunit, nu väljer den första som den hittar

            var qt = Category
                .Include(o => o.QuizUnit)
                .Where(c => c.CategoryId == id)
                .Select(c => new QuizTextVM
                {
                    CategoryName = c.Title,
                    TextHeader = c.QuizUnit.First().InfoTextHeader,
                    TextContent = c.QuizUnit.First().InfoTextContent,
                })
                .FirstOrDefault();

            return qt;
        }

        public string GetCategoryTitleById(int id)
        {
            return Category
                .SingleOrDefault(c => c.CategoryId == id)
                .Title;
        }
    }
}
