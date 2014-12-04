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

                        //var action = (string)msg.Properties["Action"];
                        if (msg.Label == "Create")
                        {
                            Stream stream = msg.GetBody<Stream>();

                            var imageId = Guid.NewGuid().ToString();
                            var contentType = msg.ContentType;

                            SaveStreamToStorage(stream, imageId, contentType);

                            var topic = (string)msg.Properties["Topic"];

                            SaveAnimalToStorage(topic, imageId, contentType); 

                        }
                        else if(msg.Label == "UpdateRating")
                        {
                            var bildId = (string)msg.Properties["BildId"];
                            var rating = (int)msg.Properties["Rating"];

                            UpdateRating(bildId, rating);
                        }
                        else if (msg.Label == "Delete")
                        {
                            var imageUri = (string)msg.Properties["ImageUri"];

                            Delete(imageUri);
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
                NumberClicks = 0,
                Created = DateTime.Now
            };

            //Sparar personen i signups table 
            TableOperation insertOperation = TableOperation.Insert(uglyAnimal);
            table.Execute(insertOperation);
        }

        private void UpdateRating(string BildId, int rating)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "UglyAnimals" table.
            CloudTable table = tableClient.GetTableReference("UglyAnimals");

            // Construct the query operation for all animals entities where PartitionKey="UglyAnimal".
            TableQuery<UglyAnimal> query = new TableQuery<UglyAnimal>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UglyAnimal"));

            //Skicka till queue med hjälp av den connectionstring vi tidigare ställt in i configen 
            QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<UglyAnimal>("UglyAnimal", BildId);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            UglyAnimal updateEntity = (UglyAnimal)retrievedResult.Result;
            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.NumberClicks += 1;
                updateEntity.TotalPoints += rating;

                // Create the InsertOrReplace TableOperation
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                // Execute the operation.
                table.Execute(insertOrReplaceOperation);

                Console.WriteLine("Entity was updated.");
            }

            else
                Console.WriteLine("Entity could not be retrieved.");

        }

        private void Delete(string imgUri)
        {
            Uri uri = new Uri(imgUri);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            CloudBlockBlob blob = container.GetBlockBlobReference(filename);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("UglyAnimals");

            // Create a retrieve operation that takes a customer entity.
            //TableOperation retrieveOperation = TableOperation.Retrieve<UglyAnimal>("BildId", "UglyRating");

            TableQuery<UglyAnimal> query = new TableQuery<UglyAnimal>().Where(TableQuery.GenerateFilterCondition("ImageId", QueryComparisons.Equal, blob.Name));

            var animal = table.ExecuteQuery(query).FirstOrDefault();

            table.Execute(TableOperation.Delete(animal));

            blob.Delete();

        }
    }
}
