using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using LearnMVC.Models.View_Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        QuizDbContext context;

        public QuizController(QuizDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Text(int id)
        {
            QuizTextVM qt = context.GetQuizTextVMById(id);
            return View(qt);
        }

        public IActionResult Question(int id)
        {
            return View();
        }
    }
}
