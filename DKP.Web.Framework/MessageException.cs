using System;

namespace DKP.Web.Framework
{
    public class MessageException : Exception
    {
        public MessageException(string message)
            :base(message)
        { 
            
        }
    }
}