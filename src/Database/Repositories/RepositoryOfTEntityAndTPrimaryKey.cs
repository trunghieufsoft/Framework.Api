using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Core.Extensions;
using Database.EntityFrameworkCore;

namespace Database.Repositories
{
    public class Repository<TEntity, TPrimary> : IRepository<TEntity, TPrimary> where TEntity : class
    {
        protected readonly APIDbContext Context;

        protected readonly DbSet<TEntity> Table;

        public Repository(IDesignTimeDbContextFactory<APIDbContext> dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext(new string[0]);
            Table = Context.Set<TEntity>();
        }

        public virtual TEntity GetById(Guid id)
        {
            return Table.Find(id);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            TEntity entity = Table.Where(where).FirstOrDefault();
            return entity;
        }

        public virtual int Count()
        {
            return Table.Count();
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public virtual IIncludableQueryable<TEntity, object> GetInclude(Expression<Func<TEntity, object>> include)
        {
            return Table.Include(include);
        }

        public virtual IQueryable<TEntity> GetInclude(string navigationPropertyPath)
        {
            return Table.Include(navigationPropertyPath);
        }

        public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return Table.Where(where);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            Table.Add(entity);
            Save();

            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Save();

            return entity;
        }

        public virtual TEntity Delete(TEntity entity)
        {
            Table.Remove(entity);
            Save();

            return entity;
        }

        public virtual List<TEntity> Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = Table.Where(where).AsEnumerable();
            List<TEntity> deletedObjects = objects.Select(Delete).ToList();

            return deletedObjects;
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            List<TEntity> listAsync = await Table.ToListAsync();
            return listAsync.AsQueryable();
        }

        public async Task<IQueryable<TEntity>> GetAllAsync(List<Expression<Func<TEntity, bool>>> expressions)
        {
            IQueryable<TEntity> data = Table;
            for (int i = 0; i < expressions.Count; i++)
            {
                data = data.Where(expressions[i]);
            }
            return (await data.ToListAsync()).AsQueryable();
        }

        public IQueryable<TEntity> GetAll(List<Expression<Func<TEntity, bool>>> expressions)
        {
            IQueryable<TEntity> data = Table;
            for (int i = 0; i < expressions.Count; i++)
            {
                data = data.Where(expressions[i]);
            }
            return (data.ToList()).AsQueryable();
        }

        public void AddOrUpdate(TEntity entity)
        {
            Table.AddOrUpdate(entity);
            Save();
        }

        public TEntity FirstOrDefaultAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.AsNoTracking().FirstOrDefault(predicate);
        }
    }
}