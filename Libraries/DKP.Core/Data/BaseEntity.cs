using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DKP.Core.Data
{
    public abstract partial class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int? CreatorId { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? UpdaterId { get; set; }

        public string UpdaterName { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}