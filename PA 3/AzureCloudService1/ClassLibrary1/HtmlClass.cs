using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ClassLibrary1
{
    public class HtmlClass: TableEntity
    {
        public HtmlClass(string url, string title, string date)
        {
            this.PartitionKey = "crawledUrl";
            this.RowKey = url;

            this.Title = title;
            this.Date = date;
        }

        public HtmlClass() { }

        public string Title { get; set; }
        public string Date { get; set; }

    }
}
