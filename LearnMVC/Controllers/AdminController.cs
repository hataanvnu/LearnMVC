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
            string memberID = userManager.GetUserId(User);
            var membersIndexVM = context.GetMembersIndexVMById(memberID, User.Identity.Name);
            membersIndexVM.SidebarVMList = context.GetSidebarVMList(memberID);
            return View(membersIndexVM);
        }
        
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public string AddCategory(AddCategoryVM model)
        {
            context.AddNewCategory(model);
            return "Det gick.";
        }
    }
}
