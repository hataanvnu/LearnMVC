using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnMVC.Models.View_Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Login(MembersLoginVM model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return $"Du är inloggad som {User.Identity.Name}";
            }
            else
            {
                return "no.";
            }
        }
    }
}
