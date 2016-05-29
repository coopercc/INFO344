using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Diagnostics;
using ClassLibrary1;
using System.Configuration;
using ClassLibrary1;
using Microsoft.WindowsAzure.Storage.Table;

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
    public class QuerySuggestions : System.Web.Services.WebService
    {
        public static Trie main; //Store Trie structure
        private string filePath = Path.GetTempPath() + "\\wiki.txt"; //Path to file holding the Wiki titkes
        private PerformanceCounter MemCounter = new PerformanceCounter("Memory", "Available MBytes");
        private static AzureConnection storageAccount = new AzureConnection(ConfigurationManager.AppSettings["StorageConnectionString"]);


        /// <summary>
        /// This method connects to the blob storage holding the wiki title file and downloads it to a local copy
        /// </summary>
        [WebMethod]
        public void ConnectBlob()
        {
            CloudBlobContainer container = storageAccount.getBlob("info344");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("titles-new.txt");

            using (var fileStream = System.IO.File.OpenWrite(filePath))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        /// <summary>
        /// BuildTrie reads the local wiki title file and creates a trie structure, stopping when the file runs out of lines
        /// or the memory remaining is below 50 MBytes.
        /// </summary>
        /// <returns> number of lines and the last word for debugging</returns>
        [WebMethod]
        public string BuildTrie()
        {
            main = new Trie();
            int count = 0;
            string lastWord = "";
            bool limit = false;

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null && !limit)
            {
                if (count % 1000 == 0 && MemCounter.NextValue() < 50)
                {
                    limit = true;
                }
                else
                {
                    main.Add(line.ToLower());
                    count++;
                    lastWord = line;
                }

            }

            file.Close();

            CloudTable stats = storageAccount.getTable("stats");
            GenStats TrieCount = new GenStats("Trie", "TrieCount", count.ToString());
            GenStats TrielastWord = new GenStats("Trie", "lastWord", lastWord);

            TableBatchOperation updateTrieStats = new TableBatchOperation();
            updateTrieStats.InsertOrReplace(TrieCount);
            updateTrieStats.InsertOrReplace(TrielastWord);
            stats.ExecuteBatch(updateTrieStats);

            return count + " " + lastWord;
        }

        /// <summary>
        /// Calls the Trie SearchPrefix class 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>List of first 10 results from the string input</returns>
        [WebMethod]
        public List<string> SearchTrie(string str)
        {
            return main.SearchPrefix(str);
        }
    }
}
