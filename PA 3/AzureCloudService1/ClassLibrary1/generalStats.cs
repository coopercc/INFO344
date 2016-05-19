using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class GenStats : TableEntity
    {
        public GenStats(string pKey, string rKey, string val)
        {
            this.PartitionKey = pKey;
            this.RowKey = rKey;
            this.val = val;
        }

        public GenStats() { }

        public string val { get; set; }

    }
}
