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
using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private AzureConnection storageAccount = new AzureConnection(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private List<string> disallow = new List<string>();
        private HashSet<string> AlreadyAdded = new HashSet<string>();
        private string urlCnn = "http://www.cnn.com";
        private string urlBR = "http://www.bleacherreport.com";

        private CloudQueue admQueue;
        private CloudQueue urlQueue;
        private CloudQueue siteMapQueue;

        private CloudTable crawled;
        private CloudTable count;

        /// <summary>
        /// 
        /// </summary>
        public override void Run()
        {
            Boolean running = false;
            admQueue = storageAccount.getQueue("admin"); //Stores Admin Messages
            urlQueue = storageAccount.getQueue("urls"); //Stores the URLS to be crawled
            siteMapQueue = storageAccount.getQueue("sitemap"); //Stores the Sitemap data to be BFS'ed
            crawled = storageAccount.getTable("crawled");

            count = storageAccount.getTable("count");
            TableOperation initCount = TableOperation.Insert(new Count(0));
            count.Execute(initCount);

            Boolean SiteMapRunning = false;

            while (true)
            {
                Thread.Sleep(100);
                CloudQueueMessage adminMessage = admQueue.GetMessage();

                if (adminMessage != null)
                {
                    string admMessage = adminMessage.AsString;
                    admQueue.DeleteMessage(adminMessage);
                    String[] msgArray = admMessage.Split(new char[] { ':' }, 2); //array[0] is start/stop msg array[1] is urls if starting
                    if (msgArray[0] == "start")
                    {
                        running = true;
                        String[] robotArray = msgArray[1].Split(',');
                        foreach (string url in robotArray)
                        {
                            WebClient wClient = new WebClient();
                            Stream data = wClient.OpenRead(url);
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
                }


                /*
                 * This is for making the sitemap work
                 * Since the sitemap is started, just handle XML
                 */
                if (running)
                {
                    CloudQueueMessage siteMapMessage = siteMapQueue.GetMessage();
                    if (siteMapMessage != null)
                    {
                        SiteMapRunning = true;
                        string message = siteMapMessage.AsString;
                        siteMapQueue.DeleteMessage(siteMapMessage);
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(message);

                        //test if cnn or bleacherreport
                        foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                        {
                            string publish = ""; //When  the last published date was. If none, then it is recent enough
                            string loc = "";
                            foreach (XmlNode locNode in node)
                            {

                                if (locNode.Name == "loc")
                                {
                                    loc = locNode.InnerText;

                                }
                                else if (locNode.Name == "lastmod")
                                {
                                    publish = locNode.InnerText;
                                }
                            }

                            if (publish == "")
                            {
                                publish = "2016-05-14";
                            }
                            DateTime dt = Convert.ToDateTime(publish);
                            CloudQueueMessage msg = new CloudQueueMessage(loc);
                            Boolean allowed = true;
                            foreach (string str in disallow)
                            {
                                if (loc.Contains(str))
                                {
                                    allowed = false;
                                }
                            }


                            if (loc.Contains(urlCnn) && dt.Year >= 2016 && dt.Month >= 3 && allowed && !AlreadyAdded.Contains(loc))
                            {

                                if (loc.EndsWith(".xml"))
                                {
                                    siteMapQueue.AddMessage(msg);
                                }
                                else if (loc.Contains(".htm"))
                                {
                                    urlQueue.AddMessage(msg);
                                    AlreadyAdded.Add(loc);
                                }
                            }
                            else if (message.Contains(urlBR))
                            {
                                urlQueue.AddMessage(msg);
                                AlreadyAdded.Add(loc);
                            }

                        }

                    } else
                    {
                        SiteMapRunning = false;
                    }

                    /*
                     * Here we will be doing the URL crawling
                     */
                    CloudQueueMessage htmlMessage = urlQueue.GetMessage();
                    if (htmlMessage != null && !SiteMapRunning)
                    {
                        string url = htmlMessage.AsString;
                        HtmlDocument htmlDoc = new HtmlWeb().Load(url);

                        string title = "";
                        string date = "05/14/2016";
                        if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                        {
                            // Handle any parse errors as required
                            //something about logging errors
                        }
                        else
                        {
                            if (htmlDoc.DocumentNode != null)
                            {
                                HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                                if (bodyNode != null)
                                {
                                    foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                                    {
                                        HtmlAttribute att = link.Attributes["href"];
                                        string newLink = att.ToString();
                                        if (newLink.Contains(urlCnn) || newLink.Contains(urlBR))
                                        {
                                            CloudQueueMessage msg = new CloudQueueMessage(newLink);
                                            urlQueue.AddMessage(msg);
                                        }
                                    }

                                    foreach (HtmlNode metas in htmlDoc.DocumentNode.SelectNodes("//meta"))
                                    {
                                        HtmlAttribute att = metas.Attributes["name"];
                                        if (att != null)
                                        {
                                            string name = att.ToString();
                                            if (name == "lastmod")
                                            {
                                                date = metas.Attributes["content"].ToString();
                                            }
                                        }
                                        HtmlAttribute att2 = metas.Attributes["property"];
                                        if (att2 != null)
                                        {
                                            string postTitle = att2.ToString();
                                            if (postTitle == "og:title")
                                            {

                                                title = metas.Attributes["content"].ToString();
                                            }
                                        }
                                    }
                                }
                                //here add to urlTable + increment count by 1
                                TableOperation insertOperation = TableOperation.Insert(new HtmlClass(url, title, date));
                                crawled.Execute(insertOperation); //add to table
                                AlreadyAdded.Add(url); //add to urls already crawled

                                //count++
                                TableOperation retrieveOperation = TableOperation.Retrieve<Count>("Count", "Count");
                                TableResult retrievedResult = count.Execute(retrieveOperation);
                                Count updateEntity = (Count)retrievedResult.Result;
                                updateEntity.count = updateEntity.count + 1;
                                TableOperation replaceOperation = TableOperation.InsertOrReplace(updateEntity);
                                count.Execute(replaceOperation);
                            }

                            /*What to do with current page:
                             *  put into crawled URL table: title, URL, Date of lastmod
                             *  
                             *  Increment count table by 1
                             */
                        }

                    }
                }
            }
        }


        //if BleacherReport, only add the nba related ones
        private void buildSiteMap(string line, string url)
        {
            string[] siteMap = line.Split(new char[] { ':' }, 2);
            if (siteMap[0] == "Sitemap")
            {
                
                CloudQueueMessage message = new CloudQueueMessage(siteMap[1].Trim());
                siteMapQueue.AddMessage(message);
            } else if (siteMap[0] == "Disallow")
            {
                disallow.Add(url + siteMap[1].Trim());
            }
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
