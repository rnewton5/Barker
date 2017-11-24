﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Barker.Models;

namespace Barker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectIfNotLoggedIn();
        }

        public IActionResult Notifications()
        {
            return RedirectIfNotLoggedIn(); // NOT YET IMPLEMENTED
        }

        public IActionResult Messages()
        {
            return RedirectIfNotLoggedIn(); // NOT YET IMPLEMENTED
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult RedirectIfNotLoggedIn()
        {
            if(!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            return View();
        }
    }
}
