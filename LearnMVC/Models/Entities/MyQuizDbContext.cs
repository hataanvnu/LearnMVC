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

            var SortedCategories = Category
                .OrderBy(c => c.Order)
                .ToArray();

            List<SidebarVM> sidebarList = new List<SidebarVM>();

            foreach (var category in SortedCategories)
            {
                sidebarList.Add(new SidebarVM
                {
                    CategoryID = category.CategoryId,
                    CategoryName = category.Title,
                    IsDone = CategoryIsDone(memberID, category.CategoryId),
                });
            }

            return sidebarList.ToArray();
        }

        private bool CategoryIsDone(string memberID, int categoryId)
        {
            var numberOfQuestionsInCategory = Question
                .Include(q => q.QuizUnit)
                .Where(q => q.QuizUnit.CategoryId == categoryId)
                .Count();

            var numberOfDoneQuestionsForUser = Progress
                .Include(p => p.Question)
                .Where(p => p.MemberId == memberID)
                .Where(p => p.Question.QuizUnit.CategoryId == categoryId)
                .Count();

            return numberOfDoneQuestionsForUser == numberOfQuestionsInCategory;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QuizUnitId"></param>
        /// <param name="memberId"></param>
        /// <returns>Returns null if there are no more questions in this quis-unit</returns>
        public QuizQuestionVM GetNextQuestion(int QuizUnitId, string memberId)
        {
            // Todo - Se till att den behandlar first, last och middle elements
            var doneQuestionsInQuizUnitForMember = Progress
                .Where(p => p.MemberId == memberId)
                .Where(p => p.Question.QuizUnitId == QuizUnitId);

            Question lastDoneQuestionInQuizUnit;
            if (doneQuestionsInQuizUnitForMember.Count() > 0)
            {
                lastDoneQuestionInQuizUnit = doneQuestionsInQuizUnitForMember
                    .Include(p => p.Question)
                    .OrderBy(p => p.Question.Order)
                    .Select(p => p.Question)
                    .Include(q => q.Answer)
                    .Last();
            }
            else
            {
                lastDoneQuestionInQuizUnit = null;
            }

            QuizQuestionVM quizQuestionVM;
            if (lastDoneQuestionInQuizUnit == null)
            {
                // Ta första frågan i QuizUnitet
                var firstQuestionOfCurrentQuizUnit = Question
                    .Where(q => q.QuizUnitId == QuizUnitId)
                    .OrderBy(q => q.Order)
                    .Include(q => q.Answer)
                    .First();

                quizQuestionVM = new QuizQuestionVM
                {
                    QuestionText = firstQuestionOfCurrentQuizUnit.QuestionText,
                    Answers = firstQuestionOfCurrentQuizUnit.Answer.ToArray(),
                    QuestionId = firstQuestionOfCurrentQuizUnit.QuestionId,
                };
            }
            else
            {
                // Leta upp de frågorna som har högre order

                var QuestionsOfHigherOrder = Question
                    .Where(q => q.QuizUnitId == QuizUnitId)
                    .Where(q => q.Order > lastDoneQuestionInQuizUnit.Order)
                    .Include(q => q.Answer);

                if (QuestionsOfHigherOrder.Count() == 0)
                {
                    // Det finns inte fler frågor i det här Quiz-unitet
                    return null;
                }
                else
                {
                    var porque = QuestionsOfHigherOrder
                        .OrderBy(q => q.Order)
                        .First();

                    quizQuestionVM = new QuizQuestionVM
                    {
                        QuestionText = porque.QuestionText,
                        Answers = porque.Answer.ToArray(),
                        QuestionId = porque.QuestionId,
                    };
                }
            }

            return quizQuestionVM;
        }

        public int GetCategoryIdByQuizUnitId(int quizUnitId)
        {
            return QuizUnit
                .Include(q => q.Category)
                .Single(q => q.QuizUnitId == quizUnitId)
                .Category
                .CategoryId;
        }

        public int GetQuizUnitIdByQuestionId(int questionId)
        {
            return Question
                .Include(q => q.QuizUnit)
                .Single(q => q.QuestionId == questionId)
                .QuizUnit
                .QuizUnitId;
        }

        public QuizTextVM GetQuizTextVMById(int categoryId, string memberId)
        {
            var nextQuizUnit = GetQuizUnit(categoryId, memberId);

            return new QuizTextVM
            {
                CategoryName = nextQuizUnit.Category.Title,
                TextContent = nextQuizUnit.InfoTextContent,
                TextHeader = nextQuizUnit.InfoTextHeader,
                QuizUnitId = nextQuizUnit.QuizUnitId,
            };
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
                return GetFirstQuizUnitInCategory(categoryId);
            }
            else
            {

                // Hämta högsta ordern på ett avklarat quizunit i current category
                int topDoneQuizUnitOrder = doneQuestionsInCategory
                    .Max(q => q.QuizUnit.Order);

                // Hämta alla quizunits med högre order.
                var nextQuizUnits = QuizUnit
                    .Where(q => q.Order > topDoneQuizUnitOrder);

                if (nextQuizUnits.Count() > 0)
                {
                    var nextQuizUnit = nextQuizUnits
                    .OrderBy(q => q.Order)
                    .Include(q => q.Category)
                    .First();

                    return nextQuizUnit;
                }

                return GetFirstQuizUnitInCategory(categoryId);
            }
        }

        private QuizUnit GetFirstQuizUnitInCategory(int categoryId)
        {
            return Category
                .Include(c => c.QuizUnit)
                .Single(c => c.CategoryId == categoryId)
                .QuizUnit
                .OrderBy(q => q.Order)
                .First();
        }

        public string GetCategoryTitleById(int categoryId)
        {
            return Category
                .SingleOrDefault(c => c.CategoryId == categoryId)
                .Title;
        }

        internal void RegisterQuestionAsCorrect(int questionId, string memberId)
        {
            Progress.Add(new Progress
            {
                MemberId = memberId,
                QuestionId = questionId,
            });

            SaveChanges();
        }

        public bool ChecksIfTheQuestionWasCorrectlyAnsweredAndInsertsThePossibleProgressToTheDatabase(int answerId, int questionId, string memberId)
        {
            var question = Question
                .Include(q => q.Answer)
                .Single(q => q.QuestionId == questionId);

            var answer = question.Answer
                .Single(a => a.AnswerId == answerId);

            if (answer.IsCorrect)
            {
                RegisterQuestionAsCorrect(questionId, memberId);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
