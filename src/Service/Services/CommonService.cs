using System;
using System.Linq;
using Entities.Entities;
using Common.DTOs.Common;
using Database.UnitOfWork;
using Database.Repositories;
using System.Collections.Generic;
using Service.Services.Abstractions;
using Entities.Enumerations;
using Common.Core.Linq.Extensions;
using Serilog;
using Common.Core.Exceptions;
using Common.Core.Enumerations;
using Common.Core.Extensions;

namespace Service.Services
{
    public class CommonService : BaseService, ICommonService
    {
        #region initial
        private readonly IUnitOfWork _unitOfWork;
        // TODO
        #endregion

        #region CommonService
        public CommonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            // TODO
        }
        #endregion

        #region API

        #endregion

        #region Method

        #endregion
    }
}
