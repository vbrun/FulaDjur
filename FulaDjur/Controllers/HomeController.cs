using FulaDjur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FulaDjur.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var fuladjur = new List<FultDjur>()
            {
                new FultDjur { Rubrik = "Fulafisken", ImageUrl = "https://24tanzania.com/wp-content/uploads/2013/09/adeaBlobfish.jpg", UglyRating = 5 },
                new FultDjur { Rubrik = "Könshår", ImageUrl = "http://www.oddee.com/_media/imgs/articles2/a97635_munchkin.jpg", UglyRating = 2 },
                new FultDjur { Rubrik = "Uppklädd", ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg", UglyRating = 4 }
            };

            return View(fuladjur);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}