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

            TableQuery<UglyComment> query = new TableQuery<UglyComment>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "UglyComment"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("AnimalId", QueryComparisons.Equal, animalId)));

            List<UglyCommentModel> uglyComments = new List<UglyCommentModel>();

            // Print the fields for each animal.
            foreach (UglyComment entity in table.ExecuteQuery(query))
            {
   
                var comment = new UglyCommentModel
                {
                    Id = entity.RowKey,
                    Name = entity.Name,
                    Text = entity.Text,
                    AnimalId = entity.AnimalId
                };

                uglyComments.Add(comment);
            }

            return uglyComments;
        }


        public void Create(UglyCommentModel comment)
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

            var bm = new BrokeredMessage();
            bm.Properties["Name"] = comment.Name;
            bm.Properties["Text"] = comment.Text;
            bm.Properties["AnimalId"] = comment.AnimalId;

            qc.Send(bm);
        }
    }
}
