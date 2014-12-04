using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Models.ViewModels
{
    public class UglyAnimalModel
    {
        public string Id { get; set; }
        public string Rubrik { get; set; }
        public string ImageUrl { get; set; }
        public float UglyRating { get; set; }
        public int NumberClicks { get; set; }
        public DateTime Created { get; set; }

        public List<UglyCommentModel> UglyComments { get; set; }
        public UglyCommentModel NewComment { get; set; }
    }
}
