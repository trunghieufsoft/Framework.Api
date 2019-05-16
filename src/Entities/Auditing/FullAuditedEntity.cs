using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Auditing
{
    public abstract class FullAuditedEntity
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }

        [Column("CREATED_USER")]
        [StringLength(2048)]
        public string CreatedBy { get; set; }

        [Column("CREATED_DT")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        [Column("LAST_UDT_USER")]
        [StringLength(2048)]
        public string LastUpdatedBy { get; set; }

        [Column("LAST_UDT_DT")]
        public DateTime? LastUpdateDate { get; set; } = DateTime.Now;
    }
}