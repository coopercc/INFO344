using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using ClassLibrary1;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading;
using System.Text;

namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        private AzureConnection storageAccount = new AzureConnection(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private CloudQueue admin;
        private CloudTable stats;
        private CloudQueue urlQueue;

        public Admin()
        {
            stats = storageAccount.getTable("stats");
            admin = storageAccount.getQueue("admin");
            urlQueue = storageAccount.getQueue("urls");
        }

        [WebMethod]
        public void Start()
        {
            CloudQueueMessage message = new CloudQueueMessage("start:http://www.cnn.com/robots.txt,http://bleacherreport.com/robots.txt");
            admin.AddMessage(message);
        }

        [WebMethod]
        public void Stop()
        {
            //Clear everything (index/table, queue/pipeline, stop all worker roles)
            CloudQueueMessage message = new CloudQueueMessage("stop");
            admin.AddMessage(message);
        }

        [WebMethod]
        public void Continue()
        {
            CloudQueueMessage message = new CloudQueueMessage("continue");
            admin.AddMessage(message);
        }
        
        [WebMethod]
        public void Clear()
        {
            //Delete all Queues and tables, reinstate them after a 40s pause
            admin.Delete();
            stats.Delete();
            urlQueue.Delete();
            storageAccount.getTable("crawled").Delete();
            storageAccount.getTable("Error").Delete();

            Thread.Sleep(40000);
            //redeclare all
            stats = storageAccount.getTable("stats");
            admin = storageAccount.getQueue("admin");
            urlQueue = storageAccount.getQueue("urls");
            CloudTable crawled = storageAccount.getTable("crawled");
            CloudTable Errors = storageAccount.getTable("Error");


        }

        //Gets number of indexed 
        [WebMethod]
        public int IndexSize()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Count>("IndexCount", "IndexCount");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return ((Count)retrievedResult.Result).count;
            } else
            {
                return 0;
            }
        }

        [WebMethod]
        public int totalCrawled()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Count>("totalCount", "totalCount");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return ((Count)retrievedResult.Result).count;
            }
            else
            {
                return 0;
            }
        }

        [WebMethod]
        public List<string> LastTen()
        {
            List<string> res = new List<string>();

            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("lastTen", "lastTen");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                string[] results = ((GenStats)retrievedResult.Result).val.Split(',');
                for(int i = 0; i <= results.Length - 1; i++)
                {
                    res.Add(results[i]);
                }
            }

            return res;
        }

        [WebMethod]
        public List<string> getErrors()
        {
            List<string> res = new List<string>();
            CloudTable ErrorTbl = storageAccount.getTable("Error");
            TableQuery<Error> errQuery = new TableQuery<Error>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,"Error"));

            foreach(Error entity in ErrorTbl.ExecuteQuery(errQuery))
            {
                res.Add(entity.Url);
            }
            return res;
        }

        [WebMethod]
        public string searchUrl(string url)
        {
            string res = "Sorry, no titles found";

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


            CloudTable crawled = storageAccount.getTable("crawled");
            TableOperation retrieveOperation = TableOperation.Retrieve<HtmlClass>("crawled", urlRow);
            TableResult retrievedResult = crawled.Execute(retrieveOperation);
            if (retrievedResult.Result  != null)
            {
                res = ((HtmlClass)retrievedResult.Result).Title;
            }

            return res;
        }

        [WebMethod]
        public string CrawlState()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("state", "state");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if ((GenStats)retrievedResult.Result != null)
            {
                return ((GenStats)retrievedResult.Result).val;
            }

            return "Idle";
        }

        [WebMethod]
        public int QueueSize()
        {
            int res = 0;
            urlQueue.FetchAttributes();
            if (urlQueue.ApproximateMessageCount != null)
            {
                res = (int)urlQueue.ApproximateMessageCount;
            }
            return res;
        }

        [WebMethod]
        public string CpuUsage()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("Usage", "cpu");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return ((GenStats)retrievedResult.Result).val;
            }
            else
            {
                return "";
            }
        }
        [WebMethod]
        public string MemUsage()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("Usage", "memory");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return ((GenStats)retrievedResult.Result).val;
            }
            else
            {
                return "";
            }
        }

    }
}
