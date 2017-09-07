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
            qt.SidebarArray = context.GetSidebarVMList(userManager.GetUserId(User));
            return View(qt);
        }


        public IActionResult Question(int id /*QuizUnitId*/)
        {
            QuizQuestionVM qqvm = context.GetNextQuestion(id, userManager.GetUserId(User));
            if (qqvm != null)
            {
                return View(qqvm);
            }
            else
            {
                int categoryId = context.getCategoryIdByQuizUnitId(id);
                return RedirectToAction(nameof(Text), new { id = categoryId });
            }
        }
    }
}
