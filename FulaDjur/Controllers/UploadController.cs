﻿using System;
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


namespace FulaDjur.Controllers
{
    public class UploadController : Controller
    {
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";
        string fuladjurstorageConnectionString = CloudConfigurationManager.GetSetting("fuladjurstorage");
       // CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("jimstoragetest"));

        // GET: Upload
        public ActionResult Index()
        {
            return View("ImageUpload");
       
        }
        
        [HttpPost]
        public ActionResult ImageUpload()
        {
            string path = @"D:\Temp\";

            var image = Request.Files["image"];
            if (image == null)
            {
                ViewBag.UploadMessage = "Failed to upload image";
            }

            else
            {
                ViewBag.UploadMessage = String.Format("Got image {0} of type {1} and size {2}",
                image.FileName, image.ContentType, image.ContentLength);

                // TODO: actually save the image to Azure blob storage
                UploadAnimal(image);

            }

            return View();
        }

        public ActionResult UploadAnimal(HttpPostedFileBase uploadedFile)
        {

            //här grejjas det
            CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to a blob named "myblob".
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));

            CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);

            blob.Properties.ContentType = uploadedFile.ContentType;
            blob.UploadFromStream(uploadedFile.InputStream);

            // Create or overwrite the "myblob" blob with contents from a local file.
            //using (var fileStream = System.IO.File.OpenRead(@"D:\testbilder\20130805_204109.jpg"))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //} 

            return View("Index");
        }
    }
}