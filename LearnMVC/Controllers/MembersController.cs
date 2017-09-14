using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LearnMVC.Models.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LearnMVC.Models.Entities;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {

        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identityContext;
        QuizDbContext context;

        public MembersController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identityContext,
            QuizDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identityContext = identityContext;
            this.context = context;
        }




        // GET: /<controller>/
        public IActionResult Index()
        {
            string memberID = userManager.GetUserId(User);
            if (context.MemberIsAdmin(memberID))
            {
                return RedirectToAction(nameof(AdminController.Index), "Admin");
            }


            var membersIndexVM = context.GetMembersIndexVMById(memberID, User.Identity.Name);

            membersIndexVM.SidebarVMList = context.GetSidebarVMList(memberID);

            return View(membersIndexVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            MembersRegisterVM model = new MembersRegisterVM
            {
                HeadersVM = new NoSidebarHeadersVM
                {
                    BigHeader = "Learn MVC!",
                    SmallHeader = "First, register:",
                }
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(MembersRegisterVM model)
        {
            #region Validera Vy-modellen
            if (!ModelState.IsValid)
                return View(model);
            #endregion


            #region Skapa Användaren
            // Skapa databastabeller
            await identityContext.Database.EnsureCreatedAsync();

            // Spara användaren i databasen
            var result = await userManager.CreateAsync(new IdentityUser(model.UserName), model.Password);
            if (!result.Succeeded)
            {
                // Lägg till fel som visas i formuläret
                ModelState.AddModelError("UserName", result.Errors.First().Description);
                return View(model);
            }

            #endregion


            #region Logga in och skicka användare vidare

            // Logga in användare (med icke-persistent cookie)
            await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            return RedirectToAction(nameof(Index));
            #endregion

        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            MembersLoginVM model = new MembersLoginVM
            {
                HeadersVM = new NoSidebarHeadersVM
                {
                    BigHeader = "Learn MVC!",
                    SmallHeader = "Nice to see you again!",
                }
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MembersLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await signInManager.SignOutAsync();

            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(MembersLoginVM.UserName), result.ToString());
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ResetProgress(bool isValidated = false)
        {
            if (!isValidated)
            {
                ResetProgressVM model = new ResetProgressVM
                {
                    SidebarVMList = context.GetSidebarVMList(userManager.GetUserId(User)),
                };
                return View(model);
            }
            else
            {
                context.ResetAllProgressForMember(userManager.GetUserId(User));
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
