using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DKP.Data.Models
{
    public class HttpModel
    {
        public string msg { get; set; }
        public int code { get; set; }
        public int count { get; set; }
        public List<Dictionary<string, object>> data { get; set; }

        public object GetValue(string field,int index = 0)
        {
            return data[index][field];
        }
    }
}
