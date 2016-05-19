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

            TableOperation retrieveOperation = TableOperation.Retrieve<Count>("lastTen", "lastTen");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                string[] results = ((GenStats)retrievedResult.Result).val.Split(',');
                for(int i = 0; i <= results.Length - 1; i++)
                {
                    res.Add(results[i]);
                }
            }
                //make call to stats table, lastTen row
                //separate by , and add to res

            return res;
        }

        [WebMethod]
        public string CrawlState()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Count>("state", "state");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            return ((GenStats)retrievedResult.Result).val;
        }

        [WebMethod]
        public int QueueSize()
        {
            int res = 0;
            if (urlQueue.ApproximateMessageCount != null)
            {
                res = (int)urlQueue.ApproximateMessageCount;
            }
            return res;
        }

        [WebMethod]
        public float CpuUsage()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("Usage", "Cpu");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return float.Parse(((GenStats)retrievedResult.Result).val, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }
        [WebMethod]
        public float MemUsage()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GenStats>("Usage", "Mem");
            TableResult retrievedResult = stats.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return float.Parse(((GenStats)retrievedResult.Result).val, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

    }
}
