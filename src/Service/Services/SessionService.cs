using Entities.Entities;
using Common.Core.Timing;
using Database.UnitOfWork;
using Entities.Enumerations;
using Database.Repositories;
using Common.Core.Exceptions;
using Common.Core.Enumerations;
using Common.Core.Linq.Extensions;
using Service.Services.Abstractions;

namespace Service.Services
{
    public class SessionService : BaseService, ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;
        private readonly IRepository<User> _userRepository;
        private readonly ISystemConfigService _configService;

        public SessionService(
            IUnitOfWork unitOfWork,
            ILogService logService,
            IRepository<User> userRepository,
            ISystemConfigService configService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _configService = configService;
            _userRepository = userRepository;
        }

        public void CheckSession(string token, string username)
        {
            User user = GetUser(username);
            if (user == default(User))
            {
                throw new DefinedException(ErrorCodeEnum.UserDeletedOrNotExisted);
            }

            //Comment for test multi browser
            //if (!user.Token.Equals(token))
            //{
            //    throw new SelfDefinedException(ErrorCodeEnum.YouHaveLogged);
            //}

            _logService.Synchronization(username);
            var config = _configService.GetSystemConfig(SystemConfigEnum.WebSessExpDate.ToString());
            var expiredDate = user.LoginTime.Value.AddMinutes((CaculateMinutesOfConfig(config)));

            if (Clock.Now < expiredDate)
            {
                user.LoginTime = Clock.Now;
                _userRepository.Update(user);

                _unitOfWork.Commit();
            }
            else
            {
                throw new DefinedException(ErrorCodeEnum.SessionExpired);
            }
        }

        #region GetUser
        public User GetUser(string username, string email = null) => _userRepository.GetAll()
                    .WhereIf(email != null, x => x.Email.Equals(email))
                    .FindField(x => x.Username.Equals(username));
        #endregion
    }
}