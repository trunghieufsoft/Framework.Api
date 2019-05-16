using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Threading.Tasks;
using Database.EntityFrameworkCore;

namespace Database.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// </summary>
        /// <param name="contextFactory">The object context</param>
        public UnitOfWork(IDesignTimeDbContextFactory<APIDbContext> contextFactory)
        {
            _dbContext = contextFactory.CreateDbContext(new string[0]);
        }

        public object Insert(object o)
        {
            _dbContext.Add(o);
            return o;
        }

        public object Update(object o)
        {
            _dbContext.Attach(o);
            _dbContext.Entry(o).State = EntityState.Modified;
            return o;
        }

        public object Delete(object o)
        {
            _dbContext.Remove(o);
            return o;
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public int Commit()
        {
            // Save changes with the default options
            int result = 0;

            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new DbUpdateException("There is an error when commit transaction.", e);
                }
            }
            return result;
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (disposing && _dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }

        public async Task<int> CommitAsync()
        {
            int result = 0;

            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new DbUpdateException("There is an error when commit transaction.", e);
                }
            }
            return result;
        }
    }
}