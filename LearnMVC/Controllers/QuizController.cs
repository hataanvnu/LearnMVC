using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using LearnMVC.Models.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        QuizDbContext context;

        public QuizController(
            UserManager<IdentityUser> userManager,
            IdentityDbContext identityContext,
            QuizDbContext context)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Text(int id /*CategoryId*/)
        {
            QuizTextVM qt = context.GetQuizTextVMById(id, userManager.GetUserId(User));
            if (qt == null)
            {
                // Det finns inga fler kategorier (eller så har något gått fel. heh.)
                return RedirectToAction(nameof(Finished));
            }
            qt.SidebarArray = context.GetSidebarVMList(userManager.GetUserId(User));
            return View(qt);
        }

        [HttpGet]
        public IActionResult Question(int id /*QuizUnitId*/)
        {
            QuizQuestionVM qqvm = context.GetNextQuestion(id, userManager.GetUserId(User));
            if (qqvm != null)
            {
                qqvm.SidebarArray = context.GetSidebarVMList(userManager.GetUserId(User));
                return View(qqvm);
            }
            else
            {
                // finns inga mer frågor i det här quiz-unitet
                int categoryId = context.GetCategoryIdByQuizUnitId(id);
                return RedirectToAction(nameof(Text), new { id = categoryId });
            }
        }

        [HttpPost]
        public IActionResult Question(int answer, int questionId)
        {
            bool isCorrect = context.ChecksIfTheQuestionWasCorrectlyAnsweredAndInsertsThePossibleProgressToTheDatabase(answer, questionId, userManager.GetUserId(User));

            if (!isCorrect)
            {
                TempData["WrongAnswer"] = "Wrong! Try Again!";
            }
            //// Användaren har svarat rätt, lägg in det i databasen och skicka till nästa fråga
            //context.registerQuestionAsCorrect(quizQuestionVM.QuestionId, userManager.GetUserId(User));

            return RedirectToAction(nameof(Question), new { id = context.GetQuizUnitIdByQuestionId(questionId) });
            //return null;
        }

        public IActionResult ResetCategory(int id /*CategoryId*/, bool isValidated = false)
        {
            if (!isValidated)
            {
                ResetCategoryVM model = new ResetCategoryVM
                {
                    CategoryId = id,
                    SidebarList = context.GetSidebarVMList(userManager.GetUserId(User)),
                };
                return View(model);
            }
            else
            {
                context.ResetCategoryForMember(id, userManager.GetUserId(User));
                return RedirectToAction(nameof(Text), new { id = id });
            }
        }

        public IActionResult Finished()
        {
            QuizFinishedVM model = new QuizFinishedVM
            {
                SidebarArray = context.GetSidebarVMList(userManager.GetUserId(User)),
            };

            return View(model);
        }
    }
}
