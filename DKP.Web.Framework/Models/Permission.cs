using System;

namespace DKP.Web.Framework.Models
{
    [Serializable]
    public class Permission
    {

        public int ID { get; set; }


        public string Name { get; set; }

        public string Title { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Url { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }

        public int Level { get; set; }

        public bool IsShow { get; set; }

        public int? Order { get; set; }

        public int? ParentId { get; set; }

    }
}