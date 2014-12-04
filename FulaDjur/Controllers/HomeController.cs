using FulaDjur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FulaDjur.Data;
using FulaDjur.Data.Implementations;
using FulaDjur.Models.ViewModels;


namespace FulaDjur.Controllers
{
    public class HomeController : Controller
    {
        IAnimalRepository _animals;
        IUglyCommentRepository _uglyComments;

        public HomeController()
        {
            _animals = new UglyAnimalRepository();
            _uglyComments = new UglyCommentRepository();
        }

        public ActionResult Index()
        {
            var model = new MainListViewModel();

            var animals = _animals.GetAll();

            foreach(var animal in animals)
            {
                var animalID = animal.Id;

                animal.UglyComments = _uglyComments.GetAll(animalID);
                animal.NewComment = new UglyCommentModel { AnimalId = animalID };
            }

            model.Animals = animals;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateComment(FormCollection form)
        {

            string animalId = Request.Form["djur.NewComment.AnimalId"];
            string name = Request.Form["djur.NewComment.Name"];
            string text = Request.Form["djur.NewComment.Text"];

            UglyCommentModel newComment = new UglyCommentModel
            {
                AnimalId = animalId,
                Name = name,
                Text = text
            };

            _uglyComments.Create(newComment);

            return RedirectToAction("Index");
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