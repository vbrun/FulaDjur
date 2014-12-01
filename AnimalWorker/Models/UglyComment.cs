using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalWorker.Models
{
    class UglyComment:TableEntity
    {
        public UglyComment(string key)
        {
            this.PartitionKey = "UglyComment";
            this.RowKey = key;
        }

        public UglyComment() { }

        public string Name { get; set; }
        public string Text { get; set; }
        public int AnimalId { get; set; }

    }

}
