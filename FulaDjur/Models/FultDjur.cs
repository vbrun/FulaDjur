using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Models
{
    public class FultDjur
    {
        public int Id { get; set; }
        public string Rubrik { get; set; }
        public string ImageUrl { get; set; }
        public int UglyRating { get; set; }

        public List<UglyComment> UglyComments { get; set; }
        public UglyComment NewComment { get; set; }
    }
}
