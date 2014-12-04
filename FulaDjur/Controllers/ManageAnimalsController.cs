using FulaDjur.Data;
using FulaDjur.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FulaDjur.Controllers
{
    [Authorize]
    public class ManageAnimalsController : Controller
    {

        IAnimalRepository _animals;

        public ManageAnimalsController()
        {
            _animals = new UglyAnimalRepository();
        }

        // GET: ManageAnimals
        public ActionResult Index()
        {
            var animals = _animals.GetAll();

            return View(animals);
        }

        public ActionResult Delete(string ImageUri)
        {
            _animals.Delete(ImageUri);
            
            ViewBag.Message = "Filen '" + ImageUri + "' kommer att tas bort...";

            var animals = _animals.GetAll();

            return View("Index", animals);
        }
    }
}