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

            StringBuilder sb = new StringBuilder();
            foreach (char c in url)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')
                    || c == ' ' || c == ',' || c == ';' || c == '-' || c == '(' || c == ')' || c == '.')
                {
                    sb.Append(c);
                }
            }

            string urlRow = sb.ToString();
            this.RowKey = urlRow;
            this.Url = url;
            this.ErrorMsg = error;
        }

        public Error() { }
        public string Url { get; set; }
        public string ErrorMsg { get; set; }

    }
}
