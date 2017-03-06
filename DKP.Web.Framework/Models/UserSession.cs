using System;
using System.Collections.Generic;

namespace DKP.Web.Framework.Models
{
    [Serializable]
    public class UserSession 
    {
        public string SessionID { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public string UserType { get; set; }

        public List<Permission> Permissions { get; set; }

        public string[] Roles { get; set; }

    }
}
