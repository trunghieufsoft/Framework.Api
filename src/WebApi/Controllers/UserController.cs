using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Core.Timing;
using Common.DTOs.Common;
using Common.DTOs.UserModel;
using Service.Services.Abstractions;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public UserController(IUserService userService, IConfiguration config, ISessionService sessionService)
        {
            _config = config;
            _userService = userService;
            _sessionService = sessionService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("WebLogin")]
        public IActionResult WebLogin([FromBody]LoginInput requestDto)
        {
            UserOutput result = _userService.WebLogin(requestDto);
            return Json(GenerateJSONWebToken(result));
        }

        [HttpPut]
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordInput requestDto)
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            _userService.ChangePassword(new DataInput<ChangePasswordInput>(requestDto, GetCurrentUser()));
            return Json(success);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword([FromBody] ResetPasswordInput requestDto)
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            _userService.ForgotPassword(new DataInput<ResetPasswordInput>(requestDto, GetCurrentUser()));
            return Json(success);
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            _userService.Logout(GetCurrentUser(), GetToken());
            return Json(success);
        }

        [HttpGet]
        [Route("KeepAlive")]
        public IActionResult KeepAlive()
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            return Json(success);
        }

        private string GenerateJSONWebToken(UserOutput userInfo)
        {
            if (userInfo == null)
            {
                return string.Empty;
            }
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            string fullName = string.IsNullOrEmpty(userInfo.FullName) ? userInfo.Username : userInfo.FullName;

            string tokenGuid = Guid.NewGuid().ToString();
            DateTime expried = Clock.Now.AddMinutes(Math.Max(Convert.ToDouble(_config["Config:TokenExpiryTimeInMinutes"]), 5));
            Claim[] claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,  userInfo.Username),
                new Claim("Username", userInfo.Username),
                new Claim("Fullname", fullName),
                new Claim("UserType", userInfo.UserType),
                new Claim("ExpiredPassword", userInfo.ExpiredPassword),
                new Claim("UserId", userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, tokenGuid)
            };

            JwtSecurityToken token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims, signingCredentials: credentials);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _userService.UpdateToken(userInfo.Id, tokenString, tokenGuid);

            return tokenString;
        }
    }
}