using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnMVC.Controllers
{
    public class HomeController : Controller
    {
        //IConfiguration conf;

        //public HomeController(IConfiguration configuration)
        //{
        //    conf = configuration;
        //}

        //// GET: /<controller>/
        //public string Index()
        //{
        //    return "connstring:" + conf.GetConnectionString("DefaultConnection");
        //}

        [Authorize]
        public string TestQuiz(int id)
        {
            return "Din senaste kategori hade id " + id;
        }
    }
}
