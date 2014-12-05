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
                    UglyRating = uglyRating,
                    Created = entity.Created
                };

                uglyAnimals.Add(animal);
            }

            var orderedAnimals = uglyAnimals.OrderByDescending(animal => animal.Created).ToList();

            return orderedAnimals;
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

            CreateQueueIfRequired();

            QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);

            var bm = new BrokeredMessage();

            bm.Label = "Delete";

            bm.Properties["ImageUri"] = imgUri;

            qc.Send(bm);

        }

        public void Create(string topic, HttpPostedFileBase file)
        {

            CreateQueueIfRequired();

            QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);

            var bm = new BrokeredMessage(file.InputStream, true);

            bm.Label = "Create";

            bm.Properties["Topic"] = topic;

            bm.ContentType = file.ContentType;

            qc.Send(bm);
        }

        public void UpdateRating(string bildId, int rating)
        {

            CreateQueueIfRequired();

            QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);

            var bm = new BrokeredMessage();

            bm.Label = "UpdateRating";

            bm.Properties["BildId"] = bildId;
            bm.Properties["Rating"] = rating;

            qc.Send(bm);
        }

        private void CreateQueueIfRequired()
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
        }
    }
}
