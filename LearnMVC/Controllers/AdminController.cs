using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnMVC.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LearnMVC.Models.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        QuizDbContext context;

        public AdminController(
            UserManager<IdentityUser> userManager,
            IdentityDbContext identityContext,
            QuizDbContext context)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //string memberID = userManager.GetUserId(User);
            //var membersIndexVM = context.GetMembersIndexVMById(memberID, User.Identity.Name);
            //membersIndexVM.SidebarVMList = context.GetSidebarVMList(memberID);
            //return View(membersIndexVM);

            QuizOverviewVM model = context.GetQuizOverviewVM();
            return View(model);
        }
        
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(AddCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            context.AddNewCategory(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddQuizUnit()
        {
            AddQuizUnitVM addQuizUnitVM = context.GetNewQuizUnitVM();
            return View(addQuizUnitVM);
        }

        [HttpPost]
        public IActionResult AddQuizUnit(AddQuizUnitVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            context.AddNewQuizUnit(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddQuestion()
        {
            AddQuestionVM addQuestionVM = context.GetNewQuestionVM();
            return View(addQuestionVM);
        }

        [HttpPost]
        public IActionResult AddQuestion(AddQuestionVM model, int correctAnswer)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            context.AddNewQuestion(model, correctAnswer);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditCategory(int id)
        {
            EditCategoryVM model = context.GetEditCategoryVMById(id);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(int id, EditCategoryVM model)
        {
            context.UpdateCategory(id, model);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult EditQuizUnit(int id)
        {
            EditQuizUnitVM model = context.GetEditQuizUnitVMById(id);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditQuizUnit(int id, EditQuizUnitVM model)
        {
            context.UpdateQuizUnit(id, model);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult EditQuestion(int id)
        {
            EditQuestionVM model = context.GetEditQuestionVMById(id);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditQuestion(int id, int correctAnswer, EditQuestionVM model)
        {
            context.UpdateQuestion(id, correctAnswer, model);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Resultat()
        {
            AdminResultatVM[] model = context.GetAdminResultatVM(userManager);

            return View(model);
        }
    }
}
