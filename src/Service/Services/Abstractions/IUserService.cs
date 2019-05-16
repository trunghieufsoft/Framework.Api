using System;
using Common.DTOs.Common;
using Common.DTOs.UserModel;

namespace Service.Services.Abstractions
{
    public interface IUserService
    {
        UserOutput WebLogin(LoginInput requestDto);
        
        void Logout(string currentUser, string token);

        void ChangePassword(DataInput<ChangePasswordInput> requestDto);

        void ForgotPassword(DataInput<ResetPasswordInput> requestDto);

        void UpdateToken(Guid id, string tokenString, string tokenGuid);
    }
}