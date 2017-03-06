using System;
using System.Collections.Generic;

namespace DKP.Core.Models
{
    [Serializable]
    public class User
    {
        public string Sid { get; set; }

        public int UserId { get; set; }

        public int InvestorId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string RealName { get; set; }

        public string Type { get; set; }

        public string Avatar { get; set; }

        public bool IsAllowEnterAdmin { get; set; }

        public bool IsSystemAdmin { get; set; }

        public List<Permission> Permissions { get; set; }

        public string[] Roles { get; set; }

        public int[] RoleIds { get; set; }

    }
}
