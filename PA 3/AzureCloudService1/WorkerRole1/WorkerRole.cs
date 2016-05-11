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

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private HashSet<string> disallow = new HashSet<string>();
        private CloudQueue admQueue;
        private CloudQueue urlQueue;
        public string urlCnn = "http://www.cnn.com";
        public string urlBR = "http://www.bleacherreport.com";

        public override void Run()
        {
            Boolean running = false;
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            admQueue = queueClient.GetQueueReference("admin");
            admQueue.CreateIfNotExists();

            urlQueue = queueClient.GetQueueReference("urls");
            urlQueue.CreateIfNotExists();

            while (true)
            {
                CloudQueueMessage adminMessage = admQueue.GetMessage();

                if (adminMessage != null)
                {
                    string message = adminMessage.AsString;
                    String[] msgArray = message.Split(':'); //array[0] is start/stop msg array[1] is urls if starting
                    if (msgArray[0] == "start")
                    {
                        running = true;
                        String[] robotArray = msgArray[1].Split(',');
                        foreach (string url in robotArray)
                        {

                            CloudQueueMessage addRobot = new CloudQueueMessage(url);
                            urlQueue.AddMessage(addRobot);
                        }

                    } else if (msgArray[0] == "stop")
                    {
                        running = false;
                    }

                    admQueue.DeleteMessage(adminMessage);
                }

                CloudQueueMessage urlMessage = urlQueue.GetMessage();
                //for running test if started
                if (urlMessage != null && running)
                {
                    string message = urlMessage.AsString;
                    /* 3 different tests: if it is a txt, then process that
                     * if .xml then process sitemap
                     * if .html then grab url date and  
                     */
                    WebClient wClient = new WebClient();
                    Stream data = wClient.OpenRead(message);
                    StreamReader read = new StreamReader(data);
                    if (!disallow.Contains(message))
                    {
                        //test if cnn or bleacherreport
                        if (message.Contains(".txt"))
                        {
                            while (read.Peek() >= 0)
                            {
                                read.ReadLine()
                            }
                            //run sitemap code
                            String[] mapArray = message.Split(':');

                            if (mapArray[0] == "Sitemap")
                            {
                                //add to url queue


                            } else if (mapArray[0] == "Disallow")
                            {
                                //add to disallow hash
                            }

                        }
                        else if (message.Contains(".html") || message.Contains(".htm"))
                        {
                            //search html page
                        }
                    }
                }

                Thread.Sleep(100);
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
