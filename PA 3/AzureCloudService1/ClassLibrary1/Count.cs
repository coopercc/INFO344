﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Count: TableEntity
    {
        public Count(string pKey, string rKey, int count)
        {
            this.PartitionKey = pKey;
            this.RowKey = rKey;
            this.count = count;
        }

        public Count() { }

        public int count { get; set; }
    }
}
