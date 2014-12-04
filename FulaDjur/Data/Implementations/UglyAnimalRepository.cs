using FulaDjur.Models.ViewModels;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AnimalWorker.Models;

namespace FulaDjur.Data.Implementations
{
    public class UglyAnimalRepository : IAnimalRepository
    {
        private string qConnectionString = "Endpoint=sb://animalqueu-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NpRLmVlZ5Gw3ChHCWBmBUYY06ZJNOTBpy2pYwoxxEso=";
        string qName = "animalqueu";

        private string fuladjurstorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=fuladjurstorage;AccountKey=0qz/KnA6q9Pcnz8FYKFzpLuW9Qde5VwUDimZUDZ5wrpYBIgPkyDBPaAgv5SwYKQCOHDNVq/LYUsiQagi1KIFxA==";

        public List<UglyAnimalModel> GetAll()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "UglyAnimals" table.
            CloudTable table = tableClient.GetTableReference("UglyAnimals");

            // Construct the query operation for all animals entities where PartitionKey="UglyAnimal".
            TableQuery<UglyAnimal> query = new TableQuery<UglyAnimal>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UglyAnimal"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            List<UglyAnimalModel> uglyAnimals = new List<UglyAnimalModel>();

            // Print the fields for each animal.
            foreach (UglyAnimal entity in table.ExecuteQuery(query))
            {
                var uglyRating = 0;

                if (entity.NumberClicks > 0)
                    uglyRating = (int)(entity.TotalPoints / entity.NumberClicks);

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(entity.ImageId);

                var animal = new UglyAnimalModel
                {
                    Id = entity.RowKey,
                    Rubrik = entity.Topic,
                    ImageUrl = blockBlob.Uri.ToString(),
                    NumberClicks = entity.NumberClicks,
                    UglyRating = uglyRating
                };

                uglyAnimals.Add(animal);
            }

            return uglyAnimals;
        }

        public float GetRating(string BildId)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("UglyAnimals");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<UglyAnimal>("BildId", "UglyRating");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Console.WriteLine(((UglyAnimalModel)retrievedResult.Result).UglyRating);
                var Result = ((UglyAnimalModel)retrievedResult.Result).UglyRating;
                return Result;
            }
            else
                Console.WriteLine("Rating culd not be recivd");
            return 0;

        }

        public void Delete(string imgUri)
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

        public void Create(string topic, HttpPostedFileBase file)
        {

            var nm = NamespaceManager.CreateFromConnectionString(qConnectionString);
            QueueDescription qd = new QueueDescription(qName);
            //Ställ in Max size på queue på  2GB 
            qd.MaxSizeInMegabytes = 2048;
            //Max Time To Live är 5 minuter   
            qd.DefaultMessageTimeToLive = new TimeSpan(0, 5, 0);
            if (!nm.QueueExists(qName))
            {
                nm.CreateQueue(qd);
            }

            //Skicka till queue med hjälp av den connectionstring vi tidigare ställt in i configen 
            QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);

            var bm = new BrokeredMessage(file.InputStream, true);

            bm.Properties["Action"] = "Create";
            bm.Properties["Topic"] = topic;

            bm.ContentType = file.ContentType;


            qc.Send(bm);
        }

        public void UpdateRating(string BildId, int counter, int rating)
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



            //var bm = new BrokeredMessage();
            //bm.Properties["Action"] = "UpdateRating";
            //bm.Properties["Rating"] = rating;

            //// In me bild här
            ////bm.Properties["Image"] =

            //qc.Send(bm);
        }
    }
}
