using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LearnMVC.Models.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {

        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identityContext;

        public MembersController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identityContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identityContext = identityContext;
        }




        // GET: /<controller>/
        public string Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return $"Välkommen {User.Identity.Name}";
            }
            else
            {
                return "User not logged in";

            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {

            return View();
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
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(MembersLoginVM model)
        {
            return View();
        }
    }
}
