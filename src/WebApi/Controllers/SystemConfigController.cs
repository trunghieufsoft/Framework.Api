using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Common.DTOs.SystemConfigModel;
using Service.Services.Abstractions;
using Entities.Enumerations;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigController : BaseController
    {
        private readonly ISystemConfigService _systemConfigService;
        private readonly ISessionService _sessionService;

        public SystemConfigController(ISystemConfigService systemConfigService, ISessionService sessionService)
        {
            _systemConfigService = systemConfigService;
            _sessionService = sessionService;
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update([FromBody] List<SystemConfigOutput> requestDto)
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            if (UserTypeEnum.SuperAdmin.Equals(GetUserTypeUser()))
            {
                _systemConfigService.Update(requestDto, GetCurrentUser());
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            _sessionService.CheckSession(GetToken(), GetCurrentUser());
            IEnumerable<SystemConfigOutput> data = UserTypeEnum.SuperAdmin.Equals(GetUserTypeUser())
                ? _systemConfigService.GetAll(GetCurrentUser())
                : null;

            return Json(data);
        }
    }
}