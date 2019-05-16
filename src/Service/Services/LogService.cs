using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Core.Enumerations;
using Common.DTOs.Common;
using Common.DTOs.LogModel;
using Database.Repositories;
using Entities.Entities;
using Service.Services.Abstractions;
using Common.Core.Timing;
using Entities.Enumerations;

namespace Service.Services
{
    public class LogService : BaseService, ILogService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Logwork> _logRepository;
        private readonly ISystemConfigService _configService;

        public LogService(IConfiguration configuration,
            IRepository<User> userRepository,
            IRepository<Logwork> logRepository,
            ISystemConfigService configService)
        {
            _configuration = configuration;
            _logRepository = logRepository;
            _configService = configService;
            _userRepository = userRepository;

            var logDB = _configuration.GetConnectionString("LogConnection");
            var logTable = "TBL_LOG_WORK";
            var options = new ColumnOptions();
            options.Store.Add(StandardColumn.LogEvent);
            options.Store.Remove(StandardColumn.MessageTemplate);
            options.Store.Remove(StandardColumn.Properties);
            options.LogEvent.DataLength = 2048;
            options.PrimaryKey = options.TimeStamp;
            options.TimeStamp.NonClusteredIndex = true;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                //.WriteTo.RollingFile("Log-{Date}.txt", retainedFileCountLimit: 2)
                .WriteTo.MSSqlServer(connectionString: logDB, tableName: logTable, columnOptions: options, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
        }

        public void System(string message)
        {
            Log.Information(message);
        }

        public void Transaction(TransactionLogEnum type, string username, string eventCode)
        {
            User user = _userRepository.GetAll().Where(x => x.Username.Equals(username)).FirstOrDefault();

            if (user != null)
            {
                TransactionLog data = new TransactionLog
                {
                    Action = type.ToString(),
                    Username = user.Username,
                    EventCode = eventCode,

                    Time = Clock.Now
                };
                Log.ForContext("Data", JsonConvert.SerializeObject(data)).Information(TypeLogEnum.TransactionLog.ToString());
            }
        }

        public void Synchronization(string username)
        {
            User user = _userRepository.GetAll().Where(x => x.Username.Equals(username)).FirstOrDefault();

            if (user != null)
            {
                SynchronizationLog data = new SynchronizationLog
                {
                    Username = user.Username,

                    Time = Clock.Now
                };
                Log.ForContext("Data", JsonConvert.SerializeObject(data)).Information(TypeLogEnum.TransactionLog.ToString());
            }
        }

        public SearchOutput Search(SearchFromTo<TypeLogEnum> requestDto, string username)
        {
            SystemConfiguration config = _configService.GetSystemConfig(SystemConfigEnum.ArchiveLogData.ToString());
            var day = CaculateDayOfConfig(config);
            var begin = Clock.Now.Date.AddDays(-day);
            if (requestDto.From != null && requestDto.From.Value > begin)
            {
                begin = requestDto.From.Value;
            }
            requestDto.From = new DateTime(begin.Year, begin.Month, begin.Day, 0, 0, 0);
            if (requestDto.To != null)
            {
                var to = requestDto.To.Value.AddDays(1);
                requestDto.To = new DateTime(to.Year, to.Month, to.Day, 0, 0, 0);
            }
            User user = _userRepository.GetAll().FirstOrDefault(x => x.Username.Equals(username));

            switch (requestDto.Property)
            {
                case TypeLogEnum.SystemLog:
                    return GetAllSystemLog(requestDto, user);

                case TypeLogEnum.SynchronizationLog:
                    return GetAllSynchronizationLog(requestDto, user);

                case TypeLogEnum.TransactionLog:
                    return GetAllTransactionLog(requestDto, user);

                default:
                    return null;
            }
        }

        private SearchOutput GetAllTransactionLog(SearchFromTo<TypeLogEnum> requestDto, User user)
        {
            List<Expression<Func<TransactionLog, bool>>> listExpresion = GetExpressions<TransactionLog>(requestDto.DataSearch, 6);
            var logQuery = _logRepository.GetAll().Where(a => TypeLogEnum.TransactionLog.ToString().Equals(a.Message) && (!requestDto.From.HasValue || a.TimeStamp >= requestDto.From.Value) && (!requestDto.To.HasValue || (a.TimeStamp < requestDto.To.Value)))
                .Select(row => new TransactionLog(row.LogEvent));
            if (listExpresion != null)
            {
                foreach (Expression<Func<TransactionLog, bool>> expression in listExpresion)
                {
                    logQuery = logQuery.Where(expression);
                }
            }
            ApplyOrderBy(requestDto.DataSearch, logQuery);
            return ApplyPaging(requestDto.DataSearch, logQuery);
        }

        private SearchOutput GetAllSynchronizationLog(SearchFromTo<TypeLogEnum> requestDto, User user)
        {
            List<Expression<Func<SynchronizationLog, bool>>> listExpresion = GetExpressions<SynchronizationLog>(requestDto.DataSearch, 5);
            var logQuery = _logRepository.GetAll().Where(a => TypeLogEnum.SynchronizationLog.ToString().Equals(a.Message) && (!requestDto.From.HasValue || a.TimeStamp >= requestDto.From.Value) && (!requestDto.To.HasValue || (a.TimeStamp < requestDto.To.Value)))
                .Select(row => new SynchronizationLog(row.LogEvent));
            if (listExpresion != null)
            {
                foreach (Expression<Func<SynchronizationLog, bool>> expression in listExpresion)
                {
                    logQuery = logQuery.Where(expression);
                }
            }
            ApplyOrderBy(requestDto.DataSearch, logQuery);
            return ApplyPaging(requestDto.DataSearch, logQuery);
        }

        private SearchOutput GetAllSystemLog(SearchFromTo<TypeLogEnum> requestDto, User user)
        {
            if (!UserTypeEnum.SuperAdmin.Equals(user.UserType))
            {
                return null;
            }
            List<Expression<Func<SystemLog, bool>>> listExpresion = GetExpressions<SystemLog>(requestDto.DataSearch, 2);
            var logQuery = _logRepository.GetAll().Where(a => !TypeLogEnum.TransactionLog.ToString().Equals(a.Message) && !TypeLogEnum.SynchronizationLog.ToString().Equals(a.Message) && (!requestDto.From.HasValue || a.TimeStamp >= requestDto.From.Value) && (!requestDto.To.HasValue || (a.TimeStamp < requestDto.To.Value)))
                .Select(row => new SystemLog { Description = row.Level, Message = row.Message, Time = row.TimeStamp });
            if (listExpresion != null)
            {
                foreach (Expression<Func<SystemLog, bool>> expression in listExpresion)
                {
                    logQuery = logQuery.Where(expression);
                }
            }
            ApplyOrderBy(requestDto.DataSearch, logQuery);
            return ApplyPaging(requestDto.DataSearch, logQuery);
        }
    }
}