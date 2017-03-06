using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DKP.Data
{
    public static class Variable
    {
        public static readonly string AppKey = ConfigurationManager.AppSettings["AppKey"].ToString();
        public static readonly string RemoteUrl = ConfigurationManager.AppSettings["RemoteUrl"].ToString();
        public static string MD5(string encryptString)
        {
            byte[] result = Encoding.Default.GetBytes(encryptString);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string encryptResult = BitConverter.ToString(output).Replace("-", "");
            return encryptResult;
        }
    }
}