using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class AzureConnection
    {
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudTableClient tableClient;

        public AzureConnection(string connect) 
        {
            storageAccount = CloudStorageAccount.Parse(connect);
            queueClient = storageAccount.CreateCloudQueueClient();
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public CloudQueue getQueue(string name)
        {
            CloudQueue queue = queueClient.GetQueueReference(name);
            queue.CreateIfNotExists();
            return queue;
        }

        public CloudTable getTable(string name)
        {
            CloudTable table = tableClient.GetTableReference(name);
            table.CreateIfNotExists();
            return table;
        }
    }
}
