using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IRepository<TEntity, in TPrimaryKey> where TEntity : class
    {
        int Count();

        TEntity GetById(Guid id);

        TEntity Get(Expression<Func<TEntity, bool>> where);

        IQueryable<TEntity> GetAll();

        Task<IQueryable<TEntity>> GetAllAsync();

        Task<IQueryable<TEntity>> GetAllAsync(List<Expression<Func<TEntity, bool>>> expressions);

        IQueryable<TEntity> GetAll(List<Expression<Func<TEntity, bool>>> expressions);

        IIncludableQueryable<TEntity, object> GetInclude(Expression<Func<TEntity, object>> include);

        IQueryable<TEntity> GetInclude(string navigationPropertyPath);

        IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);

        TEntity Insert(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Delete(TEntity entity);

        List<TEntity> Delete(Expression<Func<TEntity, bool>> where);

        void AddOrUpdate(TEntity entity);

        TEntity FirstOrDefaultAsNoTracking(Expression<Func<TEntity, bool>> predicate);

        void Save();
    }
}