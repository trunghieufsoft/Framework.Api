using System;
using Entities.Entities;
using Entities.Enumerations;

namespace Common.DTOs.UserModel
{
    #region Count Total Users
    public class CountTotalUsers
    {
        // TODO
    }
    #endregion

    public class UserOutput
    {
        public UserOutput(User user)
        {
            Id = user.Id;
            Code = user.Code;
            UserType = user.UserTypeStr;
            Username = user.Username;
            FullName = user.FullName;
            Status = user.Status;
            StatusStr = user.StatusStr;
            StartDate = user.StartDate;
            ExpiredDate = user.ExpiredDate;
            Email = user.Email;
            Address = user.Address;
            PhoneNo = user.Phone;
            InitializeInfo = new InitializeInfo(user);
        }

        #region Properties
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusStr { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string ExpiredPassword { get; set; }
        public InitializeInfo InitializeInfo { get; set; }
        #endregion
    }

    public class Status
    {
        // TODO
    }

    public class InitializeInfo
    {
        public InitializeInfo(User user)
        {
            CreatedBy = user.CreatedBy;
            CreatedDate = user.CreatedDate;
            LasUpdatedBy = user.LastUpdatedBy;
            LasUpdatedDate = user.LastUpdateDate;
            PasswordLastUpdate = user.PasswordLastUdt;
        }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LasUpdatedBy { get; set; }
        public DateTime? LasUpdatedDate { get; set; }
        public DateTime? PasswordLastUpdate { get; set; }
    }
}