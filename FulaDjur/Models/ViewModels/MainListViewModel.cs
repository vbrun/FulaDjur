using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Models.ViewModels
{
    public class MainListViewModel
    {
        public List<UglyAnimalModel> Animals { get; set; }
        public int TotalNumberOfAnimals { get; set; }
    }
}
