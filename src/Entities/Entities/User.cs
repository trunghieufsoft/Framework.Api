using System;
using Entities.Auditing;
using Entities.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entities
{
    [Table("TBL_USER")]
    public class User : AuditAuthorEntity
    {
        #region Initial
        public User()
        {

        }
        #endregion

        #region Primary key
        [Required]
        [StringLength(2048)]
        [Column("USERNAME")]
        public string Username { get; set; }
        #endregion

        #region Foreign Key References
        #endregion

        #region Properties
        [Required]
        [StringLength(128)]
        [Column("CODE")]
        public string Code { get; set; }

        [Required]
        [StringLength(2048)]
        [Column("FULL_NAME")]
        public string FullName { get; set; }

        [Required]
        [StringLength(2048)]
        [Column("USER_TYP")]
        public string UserTypeStr
        {
            get { return UserType.ToString(); }
            set { UserType = (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), value, true); }
        }

        [NotMapped]
        public UserTypeEnum UserType { get; set; }
        
        [Required]
        [StringLength(2048)]
        [Column("STATUS")]
        public string StatusStr
        {
            get { return Status.ToString(); }
            set { Status = (StatusEnum)Enum.Parse(typeof(StatusEnum), value, true); }
        }

        [NotMapped]
        public StatusEnum Status { get; set; }
        
        [Required]
        [StringLength(2048)]
        [Column("ADDRESS")]
        public string Address { get; set; }

        [StringLength(2048)]
        [Column("EMAIL")]
        public string Email { get; set; }

        [Required]
        [Column("PHONE")]
        [StringLength(128)]
        public string Phone { get; set; }

        [Column("START_DT")]
        public DateTime? StartDate { get; set; } = DateTime.Now;

        [Column("EXPIRED_DT")]
        public DateTime? ExpiredDate { get; set; } = DateTime.Now.AddMonths(6);
        #endregion

        #region Relationship

        #endregion
    }
}
