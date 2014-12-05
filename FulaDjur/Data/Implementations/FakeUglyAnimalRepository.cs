using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Data.Implementations
{
    class FakeUglyAnimalRepository : IAnimalRepository
    {

        public List<UglyAnimalModel> GetAll()
        {
            var fuladjur = new List<UglyAnimalModel>()
            {
                new UglyAnimalModel
                {
                    Id = "1",
                    Rubrik = "Fulafisken",
                    ImageUrl = "https://24tanzania.com/wp-content/uploads/2013/09/adeaBlobfish.jpg",
                    UglyRating = 5,
                    Created = DateTime.Now.AddHours(-5)
                },
                new UglyAnimalModel
                {
                    Id = "2",
                    Rubrik = "Bee happy!",
                    ImageUrl = "http://cdn1.smosh.com/sites/default/files/legacy.images/smosh-pit/122010/ugly-cat-9.jpg",
                    UglyRating = 2,
                    Created = DateTime.Now.AddHours(-1)
                },
                new UglyAnimalModel {
                    Id = "3",
                    Rubrik = "Uppklädd",
                    ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg",
                    UglyRating = 4,
                    Created = DateTime.Now.AddHours(-3)
                }
            };

            return fuladjur;
        }


        public void Create(string topic, System.Web.HttpPostedFileBase file)
        {
            throw new NotImplementedException();
        }


        public float GetRating(string BildId)
        {
            throw new NotImplementedException();
        }


        public void UpdateRating(string bildId, int rating)
        {
            throw new NotImplementedException();
        }


        public void Delete(string imgUri)
        {
            throw new NotImplementedException();
        }
    }
}
