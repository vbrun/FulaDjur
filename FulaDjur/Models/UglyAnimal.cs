using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalWorker.Models
{
    class UglyAnimal : TableEntity
    {
        public UglyAnimal(string key)
        {
            this.PartitionKey = "UglyAnimal";
            this.RowKey = key;
        }

        public UglyAnimal() { }

        public string Topic { get; set; }
        public string ImageId { get; set; }
        public int TotalPoints { get; set; }
        public int NumberClicks { get; set; }
        public DateTime Created { get; set; }

    }
}
