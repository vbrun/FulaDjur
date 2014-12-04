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
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";

        string fuladjurstorageConnectionString = CloudConfigurationManager.GetSetting("fuladjurstorage"); 

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

            List<UglyAnimalModel> uglyAnimals = new List<UglyAnimalModel>();

            // Print the fields for each animal.
            foreach (UglyAnimal entity in table.ExecuteQuery(query))
            {
                var uglyRating = 0;

                if (entity.NumberClicks > 0)
                    uglyRating = (int)(entity.TotalPoints / entity.NumberClicks);

                var animal = new UglyAnimalModel
                {
                    Id = entity.RowKey,
                    Rubrik = entity.Topic,
                    ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg",
                    NumberClicks = entity.NumberClicks,
                    UglyRating = uglyRating
                };

                uglyAnimals.Add(animal);
            }

            return uglyAnimals;
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

            var bm = new BrokeredMessage();
            bm.Properties["Topic"] = topic;

            // In me bild här
            //bm.Properties["Image"] =

            qc.Send(bm);
        }
    }
}
