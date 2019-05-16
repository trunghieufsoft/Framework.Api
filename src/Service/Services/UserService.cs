using System;
using Serilog;
using System.Linq;
using Entities.Entities;
using Common.Core.Timing;
using Common.DTOs.Common;
using Database.UnitOfWork;
using Common.Core.Services;
using Database.Repositories;
using Entities.Enumerations;
using Common.DTOs.UserModel;
using Common.Core.Extensions;
using Common.Core.Exceptions;
using Common.Core.Enumerations;
using Common.Core.Linq.Extensions;
using Service.Services.Abstractions;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;

namespace Service.Services
{
    public class UserService : BaseService, IUserService
    {
        #region initial
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;
        private readonly ISystemConfigService _configService;
        #endregion

        #region UserService
        public UserService(ILogService logService,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IConfiguration configuration,
            IRepository<User> userRepository,
            ISystemConfigService configService)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
            _configService = configService;
            _userRepository = userRepository;
        }
        #endregion

        #region API
        public UserOutput WebLogin(LoginInput requestDto)
        {
            Log.Error("Web login: {Username}/{Password}", requestDto.Username, requestDto.Password);
            User user = _userRepository.GetAll().FindField(x => x.Username.Equals(requestDto.Username));
            return Login(user, requestDto.Password);
        }

        public string GetSubcriseToken(Guid userid)
        {
            User user = _userRepository.GetById(userid);
            return user?.SubcriseToken;
        }

        public void Logout(string currentUser, string token)
        {
            User user = GetUserContact(currentUser);

            if (user != null)
            {
                user.Token = null;
                user.LoginTime = null;
                user.SubcriseToken = null;
            }
            _unitOfWork.Update(user);
            _unitOfWork.Commit();
        }

        public void UpdateToken(Guid userid, string subcriseToken, string token)
        {
            User user = _userRepository.GetById(userid);

            if (user != null)
            {
                user.Token = token;
                user.SubcriseToken = subcriseToken;
                if (string.IsNullOrEmpty(token))
                    user.LoginTime = null;
                else
                    user.LoginTime = DateTime.Now;
                _userRepository.Update(user);
                _unitOfWork.Commit();
            }
        }

        public void ChangePassword(DataInput<ChangePasswordInput> requestDto)
        {
            throw new NotImplementedException();
        }

        public void ForgotPassword(DataInput<ResetPasswordInput> requestDto)
        {
            User user = GetUserContact(requestDto.Dto.Username);
            if (user != null)
            {
                if (requestDto.Dto.Email.Trim().Equals(user.Email.Trim()))
                {
                    var diffInSeconds = Math.Round(user.PasswordLastUdt.HasValue ? (Clock.Now - user.PasswordLastUdt.Value).TotalSeconds : 300);
                    if (diffInSeconds >= 300)
                    {
                        user.PasswordLastUdt = Clock.Now;
                        user.LastUpdateDate = Clock.Now;
                        user.Password = GeneratePassword();
                        try
                        {
                            _emailService.SendForgotPassword(user.Email, EncryptService.Decrypt(user.Password), user.FullName, user.UserType.Equals(UserTypeEnum.Driver));
                            Log.Information("Reset Password For User: {Username} Successfully.", user.Username);
                            _unitOfWork.Update(user);
                            _unitOfWork.Commit();
                        }
                        catch (Exception e)
                        {
                            Log.Error(user.FullName + "reset password error {e}", e);
                            throw new DefinedException(ErrorCodeEnum.CannotSendEmailToResetPassword);
                        }
                    }
                    else
                    {
                        throw new DefinedException(ErrorCodeEnum.MultiplePasswordResetting, 300 - diffInSeconds);
                    }
                }
                else
                {
                    Log.Information("Email is incorrect!", ErrorCodeEnum.IncorrectEmail);
                    throw new DefinedException(ErrorCodeEnum.IncorrectEmail);
                }
            }
            else
            {
                Log.Information("User is incorrect!", ErrorCodeEnum.IncorrectUser);
                throw new DefinedException(ErrorCodeEnum.IncorrectUser);
            }
        }
        #endregion

        #region Method
        private UserOutput Login(User user, string password)
        {
            if (user != null)
            {
                string passEncrypt = EncryptService.Encrypt(password);
                if (user.Password.Equals(passEncrypt))
                {
                    if (user.Status != StatusEnum.Inactive && (user.LoginFailedNumber == null || (user.LoginFailedNumber != null && user.LoginFailedNumber.Value < _maxLogin)))
                    {
                        user.LoginFailedNumber = 0;
                        var userOutput = new UserOutput(user);
                        CheckExpiredPass(ref userOutput);
                        _unitOfWork.Update(user);
                        _unitOfWork.Commit();
                        _logService.Synchronization(user.Username);
                        return userOutput;
                    }
                    else
                    {
                        throw new DefinedException(ErrorCodeEnum.UserInactive);
                    }
                }
                else
                {
                    if (user.LoginFailedNumber != null && user.LoginFailedNumber.Value >= (_maxLogin - 1) && user.Status != StatusEnum.Inactive)
                    {
                        user.Status = StatusEnum.Inactive;
                        user.LoginFailedNumber = 0;
                        _unitOfWork.Update(user);
                        _unitOfWork.Commit();
                        throw new DefinedException(ErrorCodeEnum.LoginFailed3Time);
                    }
                    if (user.Status == StatusEnum.Inactive)
                    {
                        throw new DefinedException(ErrorCodeEnum.UserInactive);
                    }
                    user.LoginFailedNumber = user.LoginFailedNumber == null ? 1 : user.LoginFailedNumber.Value + 1;
                    _unitOfWork.Update(user);
                    _unitOfWork.Commit();
                    throw new DefinedException(ErrorCodeEnum.LoginFailed);
                }
            }
            else
            {
                Log.Information("User does not existed!", ErrorCodeEnum.IncorrectUser);
                throw new DefinedException(ErrorCodeEnum.IncorrectUser);
            }
        }

        private void CheckExpiredPass(ref UserOutput user)
        {
            var key = user.UserType == UserTypeEnum.Driver.ToString() ? SystemConfigEnum.AppPassExpDate.ToString() : SystemConfigEnum.WebPassExpDate.ToString();
            var config = _configService.GetSystemConfig(key);
            if (config == null || (config.LastUpdateDate.HasValue && config.LastUpdateDate.Value.Date == Clock.Now.Date))
            {
                user.ExpiredPassword = "30";
                return;
            }
            if (!user.InitializeInfo.PasswordLastUpdate.HasValue)
            {
                var account = _userRepository.GetById(user.Id);
                if (account != null)
                {
                    account.PasswordLastUdt = user.InitializeInfo.PasswordLastUpdate = Clock.Now;
                    _unitOfWork.Update(account);
                    _unitOfWork.Commit();
                }
            }
            var expiredDate = user.InitializeInfo.PasswordLastUpdate.Value.AddDays((CaculateDayOfConfig(config)));
            var now = Clock.Now;
            if (expiredDate.Date < now.Date && user.UserType == UserTypeEnum.Driver.ToString())
            {
                throw new DefinedException(ErrorCodeEnum.PasswordExpired, user.Username);
            }
            else
            {
                user.ExpiredPassword = Math.Round((expiredDate - now).TotalDays, 0).ToString();
            }
        }

        private string GeneratePassword()
        {
            int lowercase = 4;
            int uppercase = 3;
            int numerics = 2;
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return EncryptService.Encrypt(generated);
        }
        #endregion

        #region function
        private bool ExistedUser(string username)
            => _userRepository.GetAll().Any(x => x.Username.Equals(username));

        private bool ExisedEmail(string email, bool allowBlank = false, Guid? idUpdate = null)
            => !(allowBlank && string.IsNullOrEmpty(email))
                ? _userRepository.GetAll().WhereIf(idUpdate != null, x => !idUpdate.Equals(x.Id)).Any(x => x.Email.Equals(email))
                : false;

        //private bool CheckAuthority(string username, UserTypeEnum type)
        //{
        //    var userType = GetUserContact(username).UserType;
        //    switch (userType)
        //    {
        //        case UserTypeEnum.Manager:
        //        case UserTypeEnum.Staff:
        //            return (int)userType >= (int)type;

        //        case UserTypeEnum.Employee:
        //            return true;

        //        case UserTypeEnum.SuperAdmin:
        //        default:
        //            return false;
        //    }
        //}

        private User GetUserContact(string username)
            => _userRepository.Get(x => x.Username.Equals(username));
        #endregion
    }
}