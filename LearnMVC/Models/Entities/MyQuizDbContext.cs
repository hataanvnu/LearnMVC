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

        public QuizTextVM GetQuizTextVMById(int categoryId, string memberId)
        {
            // todo - lägg till logik för att välja rätt quizunit, nu väljer den första som den hittar
            var nextQuizUnit = GetQuizUnit(categoryId, memberId);

            //var qt = Category
            //    .Include(o => o.QuizUnit)
            //    .Where(c => c.CategoryId == categoryId)
            //    .Select(c => new QuizTextVM
            //    {
            //        CategoryName = c.Title,
            //        TextHeader = c.QuizUnit.First().InfoTextHeader,
            //        TextContent = c.QuizUnit.First().InfoTextContent,
            //    })
            //    .FirstOrDefault();

            return new QuizTextVM
            {
                CategoryName = nextQuizUnit.Category.Title,
                TextContent = nextQuizUnit.InfoTextContent,
                TextHeader = nextQuizUnit.InfoTextHeader,
            };

            //return qt;
        }

        private QuizUnit GetQuizUnit(int categoryId, string memberId)
        {

            // Hämta alla frågor i kategorin som användaren har klarat av
            var doneQuestionsInCategory = Progress
                .Where(p => p.MemberId == memberId)
                .Where(p => p.Question.QuizUnit.CategoryId == categoryId)
                .Select(p => p.Question)
                .Include(p => p.QuizUnit)
                .ToArray();

            if (doneQuestionsInCategory.Length == 0)
            {
                return GetFirstQuestionInCategory(categoryId);
            }
            else
            {

                // Hämta högsta ordern på ett avklarat quizunit i current category
                int topDoneQuizUnitOrder = doneQuestionsInCategory
                    .Max(q => q.QuizUnit.Order);

                // Hämta alla quizunits med högre order.
                var nextQuizUnits = doneQuestionsInCategory
                    .Where(q => q.QuizUnit.Order > topDoneQuizUnitOrder);

                if (nextQuizUnits.Count() > 0)
                {
                    var nextQuizUnit = nextQuizUnits
                    .OrderBy(q => q.QuizUnit.Order)
                    .First()
                    .QuizUnit;

                    return nextQuizUnit;
                }

                return GetFirstQuestionInCategory(categoryId);
            }
        }

        private QuizUnit GetFirstQuestionInCategory(int categoryId)
        {
            return Category
                                .Single(c => c.CategoryId == categoryId)
                                .QuizUnit
                                .OrderBy(q => q.Order)
                                .First();
        }

        public string GetCategoryTitleById(int id)
        {
            return Category
                .SingleOrDefault(c => c.CategoryId == id)
                .Title;
        }
    }
}
