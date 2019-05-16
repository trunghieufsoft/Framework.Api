using System;

namespace Database.Repositories
{
    public interface IRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class
    {
    }
}