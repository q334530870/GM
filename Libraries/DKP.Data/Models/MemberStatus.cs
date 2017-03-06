using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data.Models
{
    public class MemberStatus
    {
        public string code { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public static MemberStatus getMS(string code)
        {
            string name = "";
            string color = "";
            switch (code)
            {
                case "0":
                    name = "正常";
                    color = "primary";
                    break;
                case "1":
                    name = "锁号";
                    color = "danger";
                    break;
                case "2":
                    name = "禁言";
                    color = "warning";
                    break;
                default:
                    name = "未知";
                    color = "default";
                    break;
            }
            var ms = new MemberStatus();
            ms.code = code;
            ms.name = name;
            ms.color = color;
            return ms;
        }
    }
}
