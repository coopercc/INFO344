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

                            CloudQueueMessage urlMessage = new CloudQueueMessage(url);
                            urlQueue.AddMessage(urlMessage);
                        }

                        //for urlQueues
                        

                        //add robots.txt to url queue
                    }
                }


                /* 
                 * IDK
                 * check admin
                 * pull queue message
                 */


                    //for running test if started
                    if (retrievedMessage != null && running)
                {
                    string message = retrievedMessage.AsString;
                    /* 3 different tests: if it is a txt, then process that
                     * if .xml then process sitemap
                     * if .html then grab url date and  
                     */
                    string response = new WebClient().DownloadString(message);
                    if (!disallow.Contains(message))
                    {

                        if (message.Contains(".txt"))
                        {
                            //run sitemap code
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
