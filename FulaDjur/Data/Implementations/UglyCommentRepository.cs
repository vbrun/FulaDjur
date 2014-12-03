using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace FulaDjur.Data.Implementations
{
    public class UglyCommentRepository : IUglyCommentRepository
    {
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";

        public List<UglyCommentModel> GetAll(int id)
        {
            var allUglyComments = new List<UglyCommentModel>()
            {
                new UglyCommentModel { AnimalId = 1, Name = "Bjön", Text = "Va ful!!!"},
                new UglyCommentModel { AnimalId = 1, Name = "Pontus", Text = "OMG!"},
                new UglyCommentModel { AnimalId = 2, Name = "Vbrun", Text = "å fyfan"},
                new UglyCommentModel { AnimalId = 3, Name = "Jim", Text = "Ser ut som Björn"}
            };

            var uglyComments = allUglyComments.Where(comment => comment.AnimalId == id).ToList();

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
