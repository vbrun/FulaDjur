using FulaDjur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging; 

namespace FulaDjur.Controllers
{
    public class HomeController : Controller
    {
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";

        public ActionResult Index()
        {
            var fuladjur = new List<FultDjur>()
            {
                new FultDjur
                {
                    Id = 1,
                    Rubrik = "Fulafisken",
                    ImageUrl = "https://24tanzania.com/wp-content/uploads/2013/09/adeaBlobfish.jpg",
                    UglyRating = 5,
                    UglyComments = new List<UglyCommentModel>()
                    {
                        new UglyCommentModel { Name = "Bjön", Text = "Va ful!!!"},
                        new UglyCommentModel { Name = "Pontus", Text = "OMG!"}
                    }
                },
                new FultDjur
                {
                    Id = 2,
                    Rubrik = "Bee happy!",
                    ImageUrl = "http://cdn1.smosh.com/sites/default/files/legacy.images/smosh-pit/122010/ugly-cat-9.jpg",
                    UglyRating = 2,
                    UglyComments = new List<UglyCommentModel>()
                    {
                        new UglyCommentModel { Name = "Vbrun", Text = "å fyfan"},
                    }
                },
                new FultDjur {
                    Id = 3,
                    Rubrik = "Uppklädd",
                    ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg",
                    UglyRating = 4,
                    UglyComments = new List<UglyCommentModel>()
                    {
                        new UglyCommentModel { Name = "Jim", Text = "Ser ut som Björn"},
                    }
                }
            };

            return View(fuladjur);
        }

        [HttpPost]
        public ActionResult Index(int Id, ICollection<FultDjur> fuladjur)
        {
            var newCommentModel = fuladjur.FirstOrDefault(djur => djur.Id == Id).NewComment;

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
            bm.Properties["Name"] = newCommentModel.Name;
            bm.Properties["Text"] = newCommentModel.Text;
            bm.Properties["AnimalId"] = Id;

            qc.Send(bm);

            return RedirectToAction("Index");
        }

        public ActionResult UploadAnimal()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}