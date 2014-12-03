﻿using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Data.Implementations
{
    class FakeAnimalRepository : IAnimalRepository
    {

        public List<UglyAnimalModel> GetAll()
        {
            var fuladjur = new List<UglyAnimalModel>()
            {
                new UglyAnimalModel
                {
                    Id = 1,
                    Rubrik = "Fulafisken",
                    ImageUrl = "https://24tanzania.com/wp-content/uploads/2013/09/adeaBlobfish.jpg",
                    UglyRating = 5
                },
                new UglyAnimalModel
                {
                    Id = 2,
                    Rubrik = "Bee happy!",
                    ImageUrl = "http://cdn1.smosh.com/sites/default/files/legacy.images/smosh-pit/122010/ugly-cat-9.jpg",
                    UglyRating = 2
                },
                new UglyAnimalModel {
                    Id = 3,
                    Rubrik = "Uppklädd",
                    ImageUrl = "http://littlefun.org/uploads/524b6b7be691b20cf6c4428c_736.jpg",
                    UglyRating = 4
                }
            };

            return fuladjur;
        }
    }
}
