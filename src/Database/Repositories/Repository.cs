using System;
using Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database.Repositories
{
    public class Repository<TEntity> : Repository<TEntity, Guid>, IRepository<TEntity> where TEntity : class
    {
        public Repository(IDesignTimeDbContextFactory<APIDbContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}