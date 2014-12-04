using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using AnimalWorker.Models;

namespace FulaDjur.Data.Implementations
{
    public class UglyCommentRepository : IUglyCommentRepository
    {
        private string qConnectionString = "Endpoint=sb://animalqueu-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NpRLmVlZ5Gw3ChHCWBmBUYY06ZJNOTBpy2pYwoxxEso=";
        string qName = "commentqueu";

        private string fuladjurstorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=fuladjurstorage;AccountKey=0qz/KnA6q9Pcnz8FYKFzpLuW9Qde5VwUDimZUDZ5wrpYBIgPkyDBPaAgv5SwYKQCOHDNVq/LYUsiQagi1KIFxA==";

        public List<UglyCommentModel> GetAll(string animalId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fuladjurstorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "UglyAnimals" table.
            CloudTable table = tableClient.GetTableReference("UglyComments");

            TableQuery<UglyComment> query = new TableQuery<UglyComment>();

            if (animalId != null)
            {
                query.Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UglyComment"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("AnimalId", QueryComparisons.Equal, animalId)));
            }
            else
            {
                query.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UglyComment"));
            }

            List<UglyCommentModel> uglyComments = new List<UglyCommentModel>();

            // Print the fields for each animal.
            foreach (UglyComment entity in table.ExecuteQuery(query))
            {

                var comment = new UglyCommentModel
                {
                    Id = entity.RowKey,
                    Name = entity.Name,
                    Text = entity.Text,
                    AnimalId = entity.AnimalId,
                    Created = entity.Created
                };

                uglyComments.Add(comment);
            }

            var orderedComments = uglyComments.OrderBy(comment => comment.Created).ToList();

            return orderedComments;
        }

        public void Create(UglyCommentModel comment)
        {
            //det namn vår table ska ha 
            string tableName = "UglyComments";
            //Connection till table storage account 
            CloudStorageAccount account = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
            //Klient för table storage 
            CloudTableClient tableStorage = account.CreateCloudTableClient();
            //Hämta en reference till tablen, om inte finns, skapa table 
            CloudTable table = tableStorage.GetTableReference(tableName);
            table.CreateIfNotExists();

            var key = Guid.NewGuid().ToString();

            var newComment = new UglyComment(key)
            {
                Name = comment.Name,
                Text = comment.Text,
                AnimalId = comment.AnimalId,
                Created = DateTime.Now
            };

            //Sparar personen i signups table 
            TableOperation insertOperation = TableOperation.Insert(newComment);
            table.Execute(insertOperation);

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
