using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using System.IO;
using ClassLibrary1;
using System.Xml;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private AzureConnection storageAccount = new AzureConnection(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private HashSet<string> disallow = new HashSet<string>();
        private string urlCnn = "http://www.cnn.com";
        private string urlBR = "http://www.bleacherreport.com";
        private CloudQueue admQueue;
        private CloudQueue urlQueue;
        private CloudQueue siteMapQueue;

        /// <summary>
        /// 
        /// </summary>
        public override void Run()
        {
            Boolean running = false;
            admQueue = storageAccount.getQueue("admin"); //Stores Admin Messages
            urlQueue = storageAccount.getQueue("urls"); //Stores the URLS to be crawled
            siteMapQueue = storageAccount.getQueue("sitemap"); //Stores the Sitemap data to be BFS'ed

            while (true)
            {
                CloudQueueMessage adminMessage = admQueue.GetMessage();

                if (adminMessage != null)
                {
                    string admMessage = adminMessage.AsString;
                    String[] msgArray = admMessage.Split(':'); //array[0] is start/stop msg array[1] is urls if starting
                    if (msgArray[0] == "start")
                    {
                        running = true;
                        String[] robotArray = msgArray[1].Split(',');
                        foreach (string url in robotArray)
                        {
                            //Take care of the robots.txt here? - YES
                            WebClient wClient = new WebClient();
                            Stream data = wClient.OpenRead(admMessage);
                            StreamReader read = new StreamReader(data);

                            string line;
                            while ((line = read.ReadLine()) != null)
                            {
                                if (url.Contains(urlCnn))
                                {
                                    buildSiteMap(line, urlCnn);
                                }
                                else //for bleacherreport
                                {
                                    if (line.Contains("nba"))
                                    {
                                        buildSiteMap(line, urlBR);
                                    }

                                }
                            }
                        }

                    }
                    else if (msgArray[0] == "stop")
                    {
                        running = false;
                    }

                    admQueue.DeleteMessage(adminMessage);
                }


                /*
                 * This is for making the sitemap work
                 * Since the sitemap is started, just handle XML
                 */
                CloudQueueMessage siteMapMessage = siteMapQueue.GetMessage();
                string message = siteMapMessage.AsString;

                if (siteMapMessage != null && running)
                {


                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(message);

                    if (!disallow.Contains(message))
                    {
                        //test if cnn or bleacherreport
                        foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                        {
                            string publish = "";
                            string loc = "";
                            // first node is the url ... have to go to nexted loc node 
                            foreach (XmlNode locNode in node)
                            {
                                //IF lastMod more recent than March 1 2016
                                // thereare a couple child nodes here so only take data from node named loc 
                                if (locNode.Name == "loc")
                                {
                                    // get the content of the loc node 
                                    loc = locNode.InnerText;

                                } else if (locNode.Name == "lastmod")
                                {
                                    publish = locNode.InnerText;
                                }
                            }

                            /*
                             * if:
                             * Url is CNN, year is at least 2016, and Month is at least March
                             * OR 
                             * ORL is BleacherReport and the XML has nba in it 
                             */
                            DateTime dt = Convert.ToDateTime(publish);
                            CloudQueueMessage msg = new CloudQueueMessage(loc);
                            if ((message.Contains(urlCnn) && dt.Year >= 2016 && dt.Month >= 3))
                            {
                                
                                if (loc.EndsWith(".xml"))
                                {
                                    siteMapQueue.AddMessage(msg);
                                } else if (loc.Contains(".htm"))
                                {
                                    urlQueue.AddMessage(msg);
                                }
                            } else if (message.Contains(urlBR))
                            {
                                urlQueue.AddMessage(msg);
                            }
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }


        //if BleacherReport, only add the nba related ones
        private void buildSiteMap(string line, string url)
        {
            string[] siteMap = line.Split(':');
            if (siteMap[0] == "SiteMap")
            {
                
                CloudQueueMessage message = new CloudQueueMessage(siteMap[1].Trim());
                siteMapQueue.AddMessage(message);
            } else if (siteMap[1] == "Disallow")
            {
                disallow.Add(url + siteMap[1].Trim());
            }
        }

        private void CrawlSiteMap(XmlDocument xDoc)
        {

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
