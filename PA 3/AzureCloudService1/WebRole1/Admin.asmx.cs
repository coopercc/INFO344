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

        [WebMethod]
        public string Start()
        {

            admin = storageAccount.getQueue("admin");
            CloudQueueMessage message = new CloudQueueMessage("start:http://www.cnn.com/robots.txt,http://bleacherreport.com/robots.txt");
            admin.AddMessage(message);
            return "Success";
        }

        [WebMethod]
        public string Stop()
        {
            //Clear everything (index/table, queue/pipeline, stop all worker roles)
            CloudQueueMessage message = new CloudQueueMessage("stop");
            admin.AddMessage(message);
            return "Success";
        }

        [WebMethod]
        public string Continue()
        {
            CloudQueueMessage message = new CloudQueueMessage("continue");
            admin.AddMessage(message);
            return "Success";
        }
    }
}
