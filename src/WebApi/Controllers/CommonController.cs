using WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Entities.Enumerations;
using System;
using Common.Core.Extensions;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;

        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }
    }
}