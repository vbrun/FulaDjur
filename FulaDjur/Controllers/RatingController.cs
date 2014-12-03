using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FulaDjur.Data;
using FulaDjur.Data.Implementations;
using FulaDjur.Models.ViewModels;
using Microsoft.Ajax.Utilities;

namespace FulaDjur.Controllers
{

    public class RatingController : Controller
    {
        IAnimalRepository _animals;
        // GET: Rating
        [HttpPost]
        public void AddRatingNumbers(int number, int bildId)
        {
            _animals = new FakeAnimalRepository();
            var counter = 0;
            int i = counter++;
            var sum = + number;


            
        }

        public string GetOneAnimal(int bildId)
        {
            var animals = _animals.GetAll();

            foreach (var animal in animals)
            {
                animal.Id = bildId;

                
            }
            var result = "s";
            return result;
        }
    }
}