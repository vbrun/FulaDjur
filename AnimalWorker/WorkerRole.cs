using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Table;
using AnimalWorker.Models;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AnimalWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        string qConnectionString = "Endpoint=sb://animalqueu-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NpRLmVlZ5Gw3ChHCWBmBUYY06ZJNOTBpy2pYwoxxEso=";
        string qName = "animalqueu";

        private string fuladjurstorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=fuladjurstorage;AccountKey=0qz/KnA6q9Pcnz8FYKFzpLuW9Qde5VwUDimZUDZ5wrpYBIgPkyDBPaAgv5SwYKQCOHDNVq/LYUsiQagi1KIFxA==";

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("AnimalWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("AnimalWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("AnimalWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("AnimalWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working like an animal");

                //Skapa ny Queueclient 
                QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);
                //Ta emot det meddelande som kommer från web role.                 
                BrokeredMessage msg = qc.Receive();
                if (msg != null)
                {
                    try
                    {
                        Trace.WriteLine("Animal Received");

                        var action = (string)msg.Properties["Action"];
                        if (action == "Create")
                        {
                            Stream stream = msg.GetBody<Stream>();

                            var imageId = Guid.NewGuid().ToString();
                            var contentType = msg.ContentType;

                            SaveStreamToStorage(stream, imageId, contentType);

                            var topic = (string)msg.Properties["Topic"];

                            SaveAnimalToStorage(topic, imageId, contentType); 

                        }
                        else if(action=="UpdateRating")
                        {
                            var rating = (string)msg.Properties["Rating"];

                            //SaveAnimalToStorage(rating);
                        }

                        msg.Complete();

                    }
                    catch (Exception)
                    {
                        // Problem, lås upp message i queue 
                        msg.Abandon();
                    }
                } 

                await Task.Delay(1000);
            }
        }

        private void SaveStreamToStorage(Stream stream, string imageId, string contentType)
        {

            CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var fullName = imageId + GetEndingForType(contentType);

            CloudBlockBlob blob = container.GetBlockBlobReference(fullName);

            blob.Properties.ContentType = contentType;
            blob.UploadFromStream(stream);
        }

        private string GetEndingForType(string contentType)
        {
            if (contentType == "image/jpeg")
                return ".jpg";

            return ".bulle";
        }

        private void SaveAnimalToStorage(string topic, string imageId, string contentType)
        {
            //det namn vår table ska ha 
            string tableName = "UglyAnimals";
            //Connection till table storage account 
            CloudStorageAccount account = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
            //Klient för table storage 
            CloudTableClient tableStorage = account.CreateCloudTableClient();
            //Hämta en reference till tablen, om inte finns, skapa table
            CloudTable table = tableStorage.GetTableReference(tableName);
            table.CreateIfNotExists();

            var key = Guid.NewGuid().ToString();

            var fullName = imageId + GetEndingForType(contentType);

            var uglyAnimal = new UglyAnimal(key)
            {
                Topic = topic,
                ImageId = fullName,
                TotalPoints = 0,
                NumberClicks = 0
            };

            //Sparar personen i signups table 
            TableOperation insertOperation = TableOperation.Insert(uglyAnimal);
            table.Execute(insertOperation);
        }


        //private void SaveCommentToStorage(string name, string text, int animalid)
        //{
        //    //det namn vår table ska ha 
        //    string tableName = "UglyComments";
        //    //Connection till table storage account 
        //    CloudStorageAccount account = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
        //    //Klient för table storage 
        //    CloudTableClient tableStorage = account.CreateCloudTableClient();
        //    //Hämta en reference till tablen, om inte finns, skapa table 
        //    CloudTable table = tableStorage.GetTableReference(tableName);
        //    table.CreateIfNotExists();

        //    var key = Guid.NewGuid().ToString();

        //    var comment = new UglyComment(key)
        //    {
        //        Name = name,
        //        Text = text,
        //        AnimalId = animalid
        //    };

        //    //Sparar personen i signups table 
        //    TableOperation insertOperation = TableOperation.Insert(comment);
        //    table.Execute(insertOperation);

        //} 
    }
}
