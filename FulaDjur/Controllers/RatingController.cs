﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnimalWorker.Models;
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
        public void AddRatingNumbers(int number, string bildId)
        {
            var IdBild = bildId;
            _animals = new UglyAnimalRepository();

            var sum = + number;

           _animals.UpdateRating(IdBild,sum); 
          // var LoadRating = _animals.GetRating(bildId);
            

        }
        
        //public void updateRating(string bildId, int counter, int rating)
        //{
        //    var animals = _animals.GetAll();

        //    foreach (var animal in animals)
        //    {
        //        animal.Id = bildId;
        //        animal.UglyRating = rating;
        //        animal.NumberClicks = counter;
                

        //    }
           
        //}
       
    }
}