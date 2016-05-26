using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Node
    {
        public Node() { }

        public char Key { get; set; } //stores the character
        public Dictionary<int, Node> Children { get; set; } //stores subsequent nodes after this one.
        public bool IsWord { get; set; } //Stores whether this node is the end of a title.
    }
}
