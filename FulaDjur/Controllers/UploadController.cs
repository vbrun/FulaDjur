using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using FulaDjur.Data;
using FulaDjur.Data.Implementations;


namespace FulaDjur.Controllers
{
    public class UploadController : Controller
    {
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";
        string fuladjurstorageConnectionString = CloudConfigurationManager.GetSetting("fuladjurstorage");

        IAnimalRepository _animals;
        // CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("jimstoragetest"));

        public UploadController()
        {
            _animals = new UglyAnimalRepository();
        }

        // GET: Upload
        public ActionResult Index()
        {
            return View();
       
        }
        
        [HttpPost]
        public ActionResult Index(string rubrik)
        {

            var image = Request.Files["image"];
            if (image == null)
            {
                ViewBag.UploadMessage = "Failed to upload image";
            }

            else
            {
                ViewBag.UploadMessage = String.Format("Bilden {0} av typ {1} och storlek {2} togs emot.",
                image.FileName, image.ContentType, image.ContentLength);

                _animals.Create(rubrik, image);

            }

            return View();
        }
    }
}