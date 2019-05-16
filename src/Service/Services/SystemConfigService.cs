using System;
using System.Linq;
using System.Collections.Generic;
using Entities.Entities;
using Common.Core.Timing;
using Database.UnitOfWork;
using Database.Repositories;
using Common.Core.Exceptions;
using Common.DTOs.SystemConfigModel;
using Service.Services.Abstractions;

namespace Service.Services
{
    public class SystemConfigService : BaseService, ISystemConfigService
    {
        private readonly IRepository<SystemConfiguration> _systemConfigRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemConfigService(IRepository<SystemConfiguration> systemConfigRepository,
          IUnitOfWork unitOfWork)
        {
            _systemConfigRepository = systemConfigRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<SystemConfigOutput> GetAll(string username)
        {
            return _systemConfigRepository.GetAll().Select(x => new SystemConfigOutput(x));
        }

        public SystemConfiguration GetSystemConfig(string key)
        {
            try
            {
                return _systemConfigRepository.Get(x => x.KeyStr.Equals(key));
            }
            catch (Exception e)
            {
                throw new NotFound("Cannot get system configuration {e}", e);
            }
        }

        public void Update(List<SystemConfigOutput> requestDto, string username)
        {
            if (requestDto != null && requestDto.Count > 0)
            {
                foreach (var item in requestDto)
                {
                    var obj = GetSystemConfig(item.Key);
                    if (obj == null)
                    {
                        obj = new SystemConfiguration
                        {
                            KeyStr = item.Key,
                            ValueUnit = item.ValueUnit,
                            Value = item.Value,
                            CreatedBy = username,
                            CreatedDate = Clock.Now
                        };
                        _systemConfigRepository.Insert(obj);
                    }
                    else
                    {
                        obj.Value = item.Value;
                        obj.ValueUnit = item.ValueUnit;
                        obj.LastUpdateDate = Clock.Now;
                        obj.LastUpdatedBy = username;
                        _systemConfigRepository.Update(obj);
                        _unitOfWork.Commit();
                    }
                }
            }
        }
    }
}