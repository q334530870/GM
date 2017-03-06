using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Extensions
{
    public static class StringExtension
    {
        public static string ToSafeEmail(this string email)
        {
            var name = email.Split('@');
            var len = name[0].Length * 0.4;
            var newname = name[0].Substring(0, (int)len) + "***";
            return newname + "@" + name[1];
        }



        public static string GetRandomString(int length)
        {

            char[] constant =
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
                'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            };
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }

            return newRandom.ToString();
        }
    }
}
