using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Error : TableEntity
    {
        public Error (string url, string error)
        {
            this.PartitionKey = "Error";
            this.RowKey = "Error";

            this.Url = url;
            this.ErrorMsg = error;
        }

        public Error() { }

        public string Url { get; set; }
        public string ErrorMsg { get; set; }

    }
}
