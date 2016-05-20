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
using System.Text;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private AzureConnection storageAccount = new AzureConnection(ConfigurationManager.AppSettings["StorageConnectionString"]);

        private List<string> disallow = new List<string>();
        private HashSet<string> AlreadyAdded = new HashSet<string>();
        private HashSet<string> AlreadyCrawled = new HashSet<string>();

        private List<string> lastTen = new List<string>();
        private string CrawlState;
        private PerformanceCounter MemCounter = new PerformanceCounter("Memory", "Available MBytes");
        private PerformanceCounter CpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);

        private string urlCnn = "cnn.com";
        private string urlBR = "bleacherreport.com";

        private CloudQueue admQueue;
        private CloudQueue urlQueue;
        private Queue<string> xmls;

        private CloudTable crawled;
        private CloudTable stats;

        /// <summary>
        /// 
        /// </summary>
        public override void Run()
        {
            
            bool running = false;
            admQueue = storageAccount.getQueue("admin"); //Stores Admin Messages
            urlQueue = storageAccount.getQueue("urls"); //Stores the URLS to be crawled
            xmls = new Queue<string>();
            crawled = storageAccount.getTable("crawled");
            stats = storageAccount.getTable("stats");
            CrawlState = "Idle";


            while (true)
            {
                Thread.Sleep(10);
                CloudQueueMessage adminMessage = admQueue.GetMessage();
                if (adminMessage != null)
                {
                    string admMessage = adminMessage.AsString;
                    admQueue.DeleteMessage(adminMessage);
                    String[] msgArray = admMessage.Split(new char[] { ':' }, 2); //array[0] is start/stop msg array[1] is urls if starting
                    if (msgArray[0] == "start")
                    {
                        CrawlState = "Loading";
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
                    } else if (msgArray[0] == "continue")
                    {
                        running = true;
                    }
                }


                if (running)
                {
                    if (xmls.Count != 0)
                    {
                        string message = xmls.Dequeue();
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

                            if (loc.EndsWith("/"))
                            {
                                loc = loc + "index.html";
                            }

                            if (loc.StartsWith("/"))
                            {
                                if (message.Contains(urlCnn))
                                {
                                    loc = "www." + urlCnn + loc;
                                }
                                else if (message.Contains(urlBR)){
                                    loc = "www." + urlBR + loc;
                                }
                            }

                            if (loc.Contains(urlCnn) && dt.Year >= 2016 && dt.Month >= 3 && allowed && !AlreadyAdded.Contains(loc))
                            {

                                if (loc.EndsWith(".xml"))
                                {
                                    xmls.Enqueue(loc);
                                }
                                else if (loc.Contains(".htm"))
                                {
                                    urlQueue.AddMessageAsync(msg);
                                    AlreadyAdded.Add(loc);
                                }
                            }
                            else if (message.Contains(urlBR))
                            {
                                urlQueue.AddMessageAsync(msg);
                                AlreadyAdded.Add(loc);
                            }

                        }

                    } else
                    {
                        CrawlState = "Crawling";
                    }

                    CloudQueueMessage htmlMessage = urlQueue.GetMessage();
                    if (htmlMessage != null && CrawlState.Equals("Crawling"))
                    {
                        string url = htmlMessage.AsString;
                        urlQueue.DeleteMessage(htmlMessage);
                        
                        if (!AlreadyCrawled.Contains(url))
                        {
                            HtmlDocument htmlDoc = new HtmlWeb().Load(url);
                            string title = "";
                            string date = "05/14/2016";
                            try
                            {
                                if (htmlDoc.DocumentNode != null)
                                {
                                    HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                                    if (bodyNode != null)
                                    {
                                        foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                                        {
                                            HtmlAttribute att = link.Attributes["href"];
                                            string newLink = att.Value.ToString();
                                            if (newLink.Contains(urlCnn) || newLink.Contains(urlBR + "/articles/"))
                                            {
                                                CloudQueueMessage msg = new CloudQueueMessage(newLink);
                                                urlQueue.AddMessageAsync(msg);
                                            }
                                        }
                                    }
                                }


                                HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='pubdate']");
                                if (node != null)
                                {
                                    HtmlAttribute desc;

                                    desc = node.Attributes["content"];
                                    date = desc.Value;
                                }
                                var titleElement = htmlDoc.DocumentNode
                                                      .Element("html")
                                                      .Element("head")
                                                      .Element("title");
                                if (titleElement != null && titleElement.InnerText != "Error")
                                {
                                    title = titleElement.InnerText;
                                }
                                //here add to urlTable + increment count by 1
                                HtmlClass newPage = new HtmlClass(url, title, date);
                                TableOperation insertOperation = TableOperation.Insert(newPage);
                                crawled.Execute(insertOperation); //add to table
                                AlreadyCrawled.Add(url); //add to urls already crawled

                                //update lastTen
                                if (lastTen.Count == 10)
                                {
                                    lastTen.RemoveAt(0);

                                }
                                lastTen.Add(url);

                                //update last 10
                                string tenString = String.Join(",", lastTen.ToArray());
                                GenStats LastTenUpdate = new GenStats("lastTen", "lastTen", tenString);
                                TableOperation replaceTen = TableOperation.InsertOrReplace(LastTenUpdate);
                                stats.Execute(replaceTen);



                                //Index count++
                                TableOperation retrieveOperation = TableOperation.Retrieve<Count>("IndexCount", "IndexCount");
                                TableResult retrievedResult = stats.Execute(retrieveOperation);
                                Count updateEntity;
                                if (retrievedResult.Result != null)
                                {
                                    int ct = ((Count)retrievedResult.Result).count;
                                    updateEntity = new Count("IndexCount", "IndexCount", ct + 1);

                                }
                                else
                                {
                                    updateEntity = new Count("IndexCount", "IndexCount", 1);
                                }

                                TableOperation replaceOperation = TableOperation.InsertOrReplace(updateEntity);
                                stats.Execute(replaceOperation);
                            } catch (Exception e)
                            {
                                //add to Err table w/ E and Url
                                CloudTable ErrorTbl = storageAccount.getTable("Error");
                                TableOperation ErrorInsert = TableOperation.Insert(new Error(url, e.ToString()));
                                ErrorTbl.Execute(ErrorInsert);
                            }

                            TableOperation totalRetrieve = TableOperation.Retrieve<Count>("totalCount", "totalCount");
                            TableResult totalResult = stats.Execute(totalRetrieve);
                            Count updateTotal;
                            if (totalResult.Result != null)
                            {
                                int ct = ((Count)totalResult.Result).count;
                                updateTotal = new Count("totalCount", "totalCount", ct + 1);

                            }
                            else
                            {
                                updateTotal = new Count("totalCount", "totalCount", 1);
                            }

                            TableOperation replaceTotal = TableOperation.InsertOrReplace(updateTotal);
                            stats.Execute(replaceTotal);

                        }
                    }

                }

                GenStats crawlState = new GenStats("state", "state", CrawlState);
                TableOperation UpdateCrawl = TableOperation.InsertOrReplace(crawlState);
                stats.Execute(UpdateCrawl);

                GenStats memUsage = new GenStats("Usage", "memory", this.MemCounter.NextValue() + "");
                TableOperation UpdateMem = TableOperation.InsertOrReplace(memUsage);
                stats.Execute(UpdateMem);

                GenStats CpuUsage = new GenStats("Usage", "cpu", this.CpuCounter.NextValue() + "");
                TableOperation UpdateCpu = TableOperation.InsertOrReplace(CpuUsage);
                stats.Execute(UpdateCpu);
                
            }
            
        }


        //if BleacherReport, only add the nba related ones
        private void buildSiteMap(string line, string url)
        {
            string[] siteMap = line.Split(new char[] { ':' }, 2);
            if (siteMap[0] == "Sitemap")
            {
                xmls.Enqueue(siteMap[1].Trim());
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
