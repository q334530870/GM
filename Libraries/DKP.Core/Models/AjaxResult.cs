using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Models
{
    public class AjaxResult
    {
        public int code { get; set; }
        public string msg { get; set; }

        public AjaxResult(string msg = "操作成功",int code = 200){
            this.code = code;
            this.msg = msg;
        }
    }
}
