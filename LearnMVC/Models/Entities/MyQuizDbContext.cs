﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LearnMVC.Models.View_Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

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
                .OrderByDescending(p => p.DateCreated)
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
                // Användaren har avklarade frågor sedan innan i det här quizunitet
                lastDoneQuestionInQuizUnit = doneQuestionsInQuizUnitForMember
                    .Include(p => p.Question)
                    .OrderBy(p => p.Question.Order)
                    .Select(p => p.Question)
                    .Include(q => q.Answer)
                    .Last();
            }
            else
            {
                // Användaren har inga avklarade frågor i detta quizunit
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
                    QuizUnitHeader = QuizUnit.SingleOrDefault(q => q.QuizUnitId == QuizUnitId).InfoTextHeader,
                    CategoryTitle = QuizUnit.Include(q => q.Category).SingleOrDefault(q => q.QuizUnitId == QuizUnitId).Category.Title,
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
                        QuizUnitHeader = QuizUnit.SingleOrDefault(q => q.QuizUnitId == QuizUnitId).InfoTextHeader,
                        CategoryTitle = QuizUnit.Include(q => q.Category).SingleOrDefault(q => q.QuizUnitId == QuizUnitId).Category.Title,

                    };
                }
            }

            var categoryId = QuizUnit
                .SingleOrDefault(q => q.QuizUnitId == QuizUnitId)
                .CategoryId;

            //double numberOfQuestionsInCategory = Question
            //    .Where(q => q.QuizUnit.CategoryId == categoryId)
            //    .Include(q => q.QuizUnit)
            //    .Count();

            //double doneQuestionsInCategory = Progress
            //    //.Where(p => p.Question.QuizUnitId == QuizUnitId)
            //    .Where(p => p.Question.QuizUnit.CategoryId == categoryId)
            //    .Where(p => p.MemberId == memberId)
            //    .Include(p => p.Question)
            //    //.ThenInclude(q => q.QuizUnit)
            //    .Count();

            //quizQuestionVM.CategoryProgress = Math.Round(100.0 * doneQuestionsInCategory / numberOfQuestionsInCategory);

            quizQuestionVM.CategoryProgress = GetCategoryProgress((int)categoryId, memberId);

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

            if (nextQuizUnit == null)
            {
                return null;
            }

            if (nextQuizUnit.CategoryId != categoryId)
            {
                // Vi har gått till en ny kategori
                return new QuizTextVM
                {
                    FinishedACategory = true,
                    CategoryId = (int)nextQuizUnit.CategoryId,
                };
            }

            QuizTextVM model = new QuizTextVM
            {
                CategoryName = nextQuizUnit.Category.Title,
                TextContent = nextQuizUnit.InfoTextContent,
                TextHeader = nextQuizUnit.InfoTextHeader,
                QuizUnitId = nextQuizUnit.QuizUnitId,
                FinishedACategory = false,
                CategoryId = (int)nextQuizUnit.CategoryId,

            };

            model.CategoryProgress = GetCategoryProgress(categoryId, memberId);

            return model;
        }

        private double GetCategoryProgress(int categoryId, string memberId)
        {
            double numberOfQuestionsInCategory = Question
                            .Where(q => q.QuizUnit.CategoryId == categoryId)
                            .Include(q => q.QuizUnit)
                            .Count();

            double doneQuestionsInCategory = Progress
                //.Where(p => p.Question.QuizUnitId == QuizUnitId)
                .Where(p => p.Question.QuizUnit.CategoryId == categoryId)
                .Where(p => p.MemberId == memberId)
                .Include(p => p.Question)
                //.ThenInclude(q => q.QuizUnit)
                .Count();

            return Math.Round(100.0 * doneQuestionsInCategory / numberOfQuestionsInCategory);
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

            var numberOfQuestionsInCategory = Question
                .Where(q => q.QuizUnit.CategoryId == categoryId)
                .Count();

            if (doneQuestionsInCategory.Length == 0)
            {
                return GetFirstQuizUnitInCategory(categoryId);
            }
            else if (doneQuestionsInCategory.Length >= numberOfQuestionsInCategory)
            {
                // Alla frågor i kategorin är avklarade, navigera till nästa
                var possibleComingCategories = Category
                    .Where(c => c.Order > Category.Single(d => d.CategoryId == categoryId).Order);

                if (possibleComingCategories.Count() == 0)
                {
                    return null;
                }
                else
                {
                    var nextCategoryId = possibleComingCategories
                        .OrderBy(c => c.Order)
                        .First()
                        .CategoryId;

                    return GetQuizUnit(nextCategoryId, memberId);
                }

            }
            else
            {
                // Hämta högsta ordern på ett avklarat quizunit i current category
                int topDoneQuizUnitOrder = doneQuestionsInCategory
                    .Max(q => q.QuizUnit.Order);

                // Hämta alla quizunits med högre order.
                var nextQuizUnits = QuizUnit
                    .Where(q => q.CategoryId == categoryId)
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

        public bool MemberIsAdmin(string v)
        {
            try
            {
                return (bool)Member.Single(m => m.MemberId == v).IsAdmin;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public void AddNewCategory(AddCategoryVM model)
        {
            // Todo - fixa så att order kollas och ändras
            Category.Add(new Category
            {
                Order = Category.Max(c => c.Order) + 1,
                Title = model.CategoryTitle,
            });
            SaveChanges();
        }

        internal void AddNewQuizUnit(AddQuizUnitVM model)
        {
            // Todo - fixa så att order kollas och ändras
            QuizUnit.Add(new QuizUnit
            {
                Order = QuizUnit.Max(c => c.Order) + 1,
                InfoTextHeader = model.QuizUnitHeader,
                InfoTextContent = model.QuizUnitContent,
                CategoryId = model.SelectedCategoryId,
            });
            SaveChanges();
        }

        public AddQuizUnitVM GetNewQuizUnitVM()
        {
            AddQuizUnitVM model = new AddQuizUnitVM();

            model.Categories = Category.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.CategoryId.ToString(),
            })
            .ToArray();

            model.SelectedCategoryId = Convert.ToInt32(model.Categories[0].Value);

            return model;
        }


        internal void AddNewQuestion(AddQuestionVM model, int correctAnswer)
        {
            // Todo - fixa så att order kollas och ändras
            Question q = new Question
            {
                QuestionText = model.QuestionText,
                QuizUnitId = model.SelectedQuizUnitId,
            };
            try
            {
                q.Order = Question.Max(c => c.Order) + 1;
            }
            catch (Exception)
            {
                q.Order = 1;
            }
            Question.Add(q);

            for (int i = 0; i < model.Answers.Length; i++)
            {
                Answer.Add(new Answer
                {
                    AnswerText = model.Answers[i],
                    IsCorrect = i == correctAnswer,
                    Question = q,
                });
            }
            SaveChanges();
        }

        public AddQuestionVM GetNewQuestionVM()
        {
            AddQuestionVM model = new AddQuestionVM();

            model.PossibleQuizUnits = QuizUnit.Select(c => new SelectListItem
            {
                Text = c.InfoTextHeader,
                Value = c.QuizUnitId.ToString(),
            })
            .ToArray();

            model.SelectedQuizUnitId = Convert.ToInt32(model.PossibleQuizUnits[0].Value);

            model.Answers = new string[4];

            return model;
        }

        internal void ResetCategoryForMember(int categoryId, string memberId)
        {
            var q = Progress
                .Where(p => p.Question.QuizUnit.CategoryId == categoryId)
                .Where(p => p.MemberId == memberId);

            foreach (var item in q)
            {
                Progress.Remove(item);
            }
            SaveChanges();
        }

        internal void ResetAllProgressForMember(string memberId)
        {
            var q = Progress
                .Where(p => p.MemberId == memberId);

            foreach (var item in q)
            {
                Progress.Remove(item);
            }
            SaveChanges();
        }

        internal QuizOverviewVM GetQuizOverviewVM()
        {

            List<Category> que = new List<Category>();
            foreach (var category in Category)
            {
                List<QuizUnit> porque = new List<QuizUnit>();
                foreach (var quizUnit in QuizUnit)
                {
                    List<Question> donde = new List<Question>();
                    foreach (var question in Question)
                    {
                        if (question.QuizUnitId == quizUnit.QuizUnitId)
                        {
                            donde.Add(question);
                        }
                    }
                    if (quizUnit.CategoryId == category.CategoryId)
                    {
                        porque.Add(quizUnit);
                    }
                }
                que.Add(category);
            }


            QuizOverviewVM model = new QuizOverviewVM
            {
                Categories = que.ToArray(),
                NumberOfCategories = Category.Count(),
                NumberOfQuizUnits = QuizUnit.Count(),
                NumberOfQuestions = Question.Count(),
            };

            return model;
        }

        internal EditCategoryVM GetEditCategoryVMById(int id)
        {
            var q = Category
                .SingleOrDefault(c => c.CategoryId == id);

            EditCategoryVM model = new EditCategoryVM
            {
                CategoryTitle = q.Title,
                Order = q.Order,
            };

            return model;
        }

        internal void UpdateCategory(int id, EditCategoryVM newModel)
        {
            var oldCategory = Category.SingleOrDefault(c => c.CategoryId == id);

            oldCategory.Title = newModel.CategoryTitle;
            oldCategory.Order = newModel.Order;

            SaveChanges();
        }

        internal EditQuizUnitVM GetEditQuizUnitVMById(int id)
        {
            var q = QuizUnit
                .SingleOrDefault(c => c.QuizUnitId == id);

            EditQuizUnitVM model = new EditQuizUnitVM
            {
                QuizUnitHeader = q.InfoTextHeader,
                QuizUnitContent = q.InfoTextContent,
                Order = q.Order,
                SelectedCategoryId = (int)q.CategoryId,
                Categories = Category.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.CategoryId.ToString(),
                })
                .ToArray(),
            };

            return model;
        }

        internal void UpdateQuizUnit(int id, EditQuizUnitVM newModel)
        {
            var oldQuizUnit = QuizUnit
                .Single(q => q.QuizUnitId == id);

            oldQuizUnit.InfoTextHeader = newModel.QuizUnitHeader;
            oldQuizUnit.InfoTextContent = newModel.QuizUnitContent;
            oldQuizUnit.Order = newModel.Order;
            oldQuizUnit.CategoryId = newModel.SelectedCategoryId;

            SaveChanges();
        }

        internal EditQuestionVM GetEditQuestionVMById(int id)
        {
            var q = Question
                .Include(c => c.Answer)
                .SingleOrDefault(c => c.QuestionId == id);

            EditQuestionVM model = new EditQuestionVM
            {
                Answers = q.Answer.Select(a => a.AnswerText).ToArray(),
                Order = q.Order,
                QuestionText = q.QuestionText,
                SelectedQuizUnitId = (int)q.QuizUnitId,
                PossibleQuizUnits = QuizUnit.Select(c => new SelectListItem
                {
                    Text = c.InfoTextHeader,
                    Value = c.QuizUnitId.ToString(),
                })
                .ToArray(),
            };
            return model;
        }

        internal void UpdateQuestion(int id, int correctAnswer, EditQuestionVM newModel)
        {
            var oldQuestion = Question.Include(q => q.Answer).SingleOrDefault(q => q.QuestionId == id);

            oldQuestion.Order = newModel.Order;
            oldQuestion.QuestionText = newModel.QuestionText;
            oldQuestion.QuizUnitId = newModel.SelectedQuizUnitId;



            foreach (var item in oldQuestion.Answer)
            {
                Answer.Remove(Answer.Single(a => a.AnswerId == item.AnswerId));
            }


            for (int i = 0; i < newModel.Answers.Length; i++)
            {
                oldQuestion.Answer.Add(new Answer
                {
                    AnswerText = newModel.Answers[i],
                    IsCorrect = i == correctAnswer,
                    Question = oldQuestion,
                });
            }

            SaveChanges();
        }


        public ProgressVM GetProgressVM(string memberId)
        {
            ProgressVM model = new ProgressVM
            {
                DoneQuestions = Progress
                .Where(p => p.MemberId == memberId)
                .Count(),

                TotalAmountOfQuestion = Question.Count(),
            };

            return model;
        }


        internal AdminResultatVM[] GetAdminResultatVM(UserManager<IdentityUser> userManager)
        {
            List<AdminResultatVM> adminResultatList = new List<AdminResultatVM>();

            var totalNumberOfQuestions = Question.Count();




            var listOfMember = userManager.Users.Select(u => new { u.UserName, u.Id });


            foreach (var member in listOfMember)
            {
                double q = Progress
                    .Where(p => p.MemberId == member.Id)
                    .Count();

                adminResultatList.Add(new AdminResultatVM
                {
                    Name = member.UserName,
                    ProgressPercentage = Math.Round(100.0 * q / totalNumberOfQuestions),
                });
            }

            return adminResultatList.ToArray();
        }
    }
}
