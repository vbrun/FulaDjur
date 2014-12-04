using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FulaDjur.Data
{
    public interface IAnimalRepository
    {
        List<UglyAnimalModel> GetAll();
        float GetRating(string BildId);
        void Create(string topic, HttpPostedFileBase image);
        void UpdateRating(string bildId, int counter, int rating);
    }
}
