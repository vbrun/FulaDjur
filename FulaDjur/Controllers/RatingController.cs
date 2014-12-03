using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace FulaDjur.Controllers
{
    public class RatingController : Controller
    {
        // GET: Rating
        public ActionResult AddRatingNumbers(int number)
        {
            var sum = + number;
            return View();
        }
    }
}