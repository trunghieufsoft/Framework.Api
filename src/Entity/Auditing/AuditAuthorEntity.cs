using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Auditing
{
    public abstract class AuditAuthorEntity : FullAuditedEntity
    {
        [Column("LOGIN_FAILED_NR")]
        public int? LoginFailedNumber { get; set; }

        [Column("TOKEN")]
        [StringLength(2048)]
        public string Token { get; set; }

        [Column("SUBCRISE_TOKEN")]
        [StringLength(2048)]
        public string SubcriseToken { get; set; }

        [Column("TOKEN_EXPIRED_DT")]
        public DateTime? TokenExpiredDate { get; set; }

        [Column("LOGIN_TM")]
        public DateTime? LoginTime { get; set; }

        [Required]
        [Column("PASSWORD")]
        [StringLength(1024)]
        public string Password { get; set; }

        [Column("PASSWORD_LAST_UDT")]
        public DateTime? PasswordLastUdt { get; set; } = DateTime.Now;
    }
}
