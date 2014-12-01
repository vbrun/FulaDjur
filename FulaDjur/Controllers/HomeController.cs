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
                new FultDjur
                {
                    Id = 1,
                    Rubrik = "Fulafisken",
                    ImageUrl = "https://24tanzania.com/wp-content/uploads/2013/09/adeaBlobfish.jpg",
                    UglyRating = 5,
                    UglyComments = new List<UglyComment>()
                    {
                        new UglyComment { Name = "Bjön", Text = "Va ful!!!"},
                        new UglyComment { Name = "Pontus", Text = "OMG!"}
                    }
                },
                new FultDjur
                {
                    Id = 2,
                    Rubrik = "Bee happy!",
                    ImageUrl = "http://cdn1.smosh.com/sites/default/files/legacy.images/smosh-pit/122010/ugly-cat-9.jpg",
                    UglyRating = 2,
                    UglyComments = new List<UglyComment>()
                    {
                        new UglyComment { Name = "Vbrun", Text = "å fyfan"},
                    }
                },
                new FultDjur {
                    Id = 3,
                    Rubrik = "Uppklädd",
                    ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg",
                    UglyRating = 4,
                    UglyComments = new List<UglyComment>()
                    {
                        new UglyComment { Name = "Jim", Text = "Ser ut som Björn"},
                    }
                }
            };

            return View(fuladjur);
        }

        [HttpPost]
        public ActionResult Index(int Id, ICollection<FultDjur> fuladjur)
        {
            var newComment = fuladjur.FirstOrDefault(djur => djur.Id == Id).NewComment;

            return View();
        }

        public ActionResult UploadAnimal()
        {
            return View();
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