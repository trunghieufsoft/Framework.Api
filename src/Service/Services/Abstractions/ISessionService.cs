using Entities.Entities;
using System;

namespace Service.Services.Abstractions
{
    public interface ISessionService
    {
        void CheckSession(string token, string username);
        User GetUser(string username, string email = null);
    }
}