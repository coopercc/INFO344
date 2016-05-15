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

            StringBuilder sb = new StringBuilder();
            foreach (char c in url)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')        
                    || c == ' ' || c == ',' || c == ';' || c == '-' || c == '(' || c == ')' || c == '.')
                {
                    sb.Append(c);
                }
            }
            string rowUrl = sb.ToString();

            this.RowKey = rowUrl;

            this.Title = title;
            this.Date = date;
            this.Url = url;
            DateTime Timestamp = DateTime.Now;
        }

        public HtmlClass() { }

        public string Title { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }

    }
}
