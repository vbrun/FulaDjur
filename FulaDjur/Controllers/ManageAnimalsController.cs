using FulaDjur.Data;
using FulaDjur.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FulaDjur.Controllers
{
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
    }
}