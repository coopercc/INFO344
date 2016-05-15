using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClassLibrary1
{
    public class HtmlClass : TableEntity
    {
        public HtmlClass(string url, string title, string date)
        {
            this.PartitionKey = "crawled";
            this.RowKey = title;

            this.Url = url;
            this.Date = date;
        }

        public HtmlClass() { }

        public string Url { get; set; }
        public string Date { get; set; }

    }
}
