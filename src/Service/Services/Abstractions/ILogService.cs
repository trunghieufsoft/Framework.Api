using Common.Core.Enumerations;
using Common.DTOs.Common;

namespace Service.Services.Abstractions
{
    public interface ILogService
    {
        void Transaction(TransactionLogEnum type, string username, string JobId);
        void Synchronization(string username);
        SearchOutput Search(SearchFromTo<TypeLogEnum> requestDto, string user);
    }
}