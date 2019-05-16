using System.Linq;
using Entities.Entities;
using Common.Core.Timing;
using Common.Core.Services;
using Entities.Enumerations;
using Common.Core.Extensions;
using System.Threading.Tasks;
using Common.Core.Enumerations;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Database.EntityFrameworkCore
{
    public class DbInitializer
    {
        private readonly APIDbContext _context;
        private readonly IConfiguration _configuration;

        public DbInitializer(APIDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Seed()
        {
            var change = false;

            if (!_context.Users.Any(x => x.UserType == UserTypeEnum.SuperAdmin))
            {
                List<User> listUser = new List<User>()
                {
                    new User(){Code = EnumIDGenerate.SuperAdmin.GenerateCode(100001),UserType=UserTypeEnum.SuperAdmin,Username=_configuration["SuperAdmin:Username"],FullName=_configuration["SuperAdmin:Fullname"],Phone=_configuration["SuperAdmin:Default"],Email=_configuration["SuperAdmin:Email"],Address=_configuration["SuperAdmin:Default"],Status=StatusEnum.Active,Password=EncryptService.Encrypt(_configuration["SuperAdmin:Password"]),CreatedBy=_configuration["Auto:Create"]},
                    new User(){Code = EnumIDGenerate.SuperAdmin.GenerateCode(100002),UserType=UserTypeEnum.SuperAdmin,Username="string",FullName="SuperAdmin",Phone="NA",Email=_configuration["SuperAdmin:Email"],Address=_configuration["SuperAdmin:Default"],Status=StatusEnum.Active,Password=EncryptService.Encrypt("string"),CreatedBy=_configuration["Auto:Create"]},
                };
                _context.Users.AddRange(listUser);
                change = true;
            }

            IList<SystemConfiguration> defaultConfig = new List<SystemConfiguration>();
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.ArchiveJobHistory, Value = "2", Unit = Unit.months });
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.ArchiveLogData, Value = "2", Unit = Unit.weeks });
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.WebPassExpDate, Value = "30", Unit = Unit.days });
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.AppPassExpDate, Value = "30", Unit = Unit.minutes });
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.WebSessExpDate, Value = "45", Unit = Unit.minutes });
            defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.AppSessExpDate, Value = "45", Unit = Unit.minutes });
            //defaultConfig.Add(item: new SystemConfiguration { Key = SystemConfigEnum.OpenEAM, Value = "false", Unit = Unit.boolean });
            foreach (var item in defaultConfig)
            {
                if (!_context.SystemConfigurations.Any(x => x.Key == item.Key))
                {
                    _context.SystemConfigurations.Add(new SystemConfiguration
                    {
                        CreatedBy = _configuration["Auto:Create"],
                        Value = item.Value,
                        Unit = item.Unit,
                        Key = item.Key,
                        CreatedDate = Clock.Now
                    });
                    change = true;
                }
            }

            if (change)
            {
                _context.SaveChanges();
            }
        }
    }
}
