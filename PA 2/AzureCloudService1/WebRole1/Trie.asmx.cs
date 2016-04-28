using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

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
    public class Trie
    {
        private Node root; //The rootNode that the Trie is built from
        private int maxCount = 10; //Max number of results to return

        public Trie()
        {
            root = new Node() {};
        }

        /// <summary>
        /// Adds the input word to the Trie. Creates new Nodes if there is no character node that matches.
        /// </summary>
        /// <param name="word"></param>
        public void Add(string word)
        {
            Node curr = root;
            Node temp = null;
            foreach (char ch in word)
            {
                if (curr.Children == null)
                {
                    curr.Children = new Dictionary<int, Node>();
                }

                if (!curr.Children.Keys.Contains(ch))
                {
                    temp = new Node() { Key = ch };
                    curr.Children.Add(ch, temp);
                }

                curr = curr.Children[ch];
            }
            curr.IsWord = true;
        }

        /// <summary>
        /// Searches the Trie looking for first 10 words that start with the input string.
        /// Calls GetMore in order to search the Trie by depth
        /// </summary>
        /// <param name="str">What was inputted into the textbox</param>
        /// <returns>List of strings of matching words</returns>
        public List<string> SearchPrefix(string str)
        {
            List<string> res = new List<string>();
            if (str == "")
            {
                return res;
            }
            Node curr = root;
            string prefix = "";
            bool fail = false;

            foreach (char ch in str)
            {
                if (curr.Children == null)
                {
                    fail = true;
                    break;
                }

                if (curr.Children.Keys.Contains(ch))
                {
                    prefix += ch;
                    curr = curr.Children[ch];
                }
                else
                {
                    fail = true;
                    break;
                }
            }

            if (!fail)
            {
                if (curr.IsWord)
                {
                    res.Add(prefix);
                }
                res = GetMore(curr, res, prefix);

            }
            return res;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curr">The current Node</param>
        /// <param name="res">The results</param>
        /// <param name="prefix">what characters have been searched so far</param>
        /// <returns>a list of strings that match the prefix</returns>
        private List<string> GetMore(Node curr, List<string> res, String prefix)
        {
            if (curr.Children == null)
            {
                return res;
            }

            foreach (Node node in curr.Children.Values)
            {
                string temp = prefix + node.Key;
                res = GetMore(node, res, temp);
                if (node.IsWord)
                {
                    if (res.Count >= maxCount)
                    {
                        return res;
                    }
                    else
                    {
                        res.Add(temp);
                    }

                }

            }
            return res;
        }
    }
}
