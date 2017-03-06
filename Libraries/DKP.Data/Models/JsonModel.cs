using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data.Models
{
    public class JsonModel
    {
        public int total { get; set; }
        public IList<Dictionary<string,object>> rows { get; set; }
        public bool status { get; set; }
        public string msg { get; set; }
    }
}
