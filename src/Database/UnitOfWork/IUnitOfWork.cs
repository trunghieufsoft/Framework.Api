using System;
using System.Threading.Tasks;

namespace Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        object Insert(object o);

        object Update(object o);

        object Delete(object o);

        /// <summary>
        ///     Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();

        Task<int> CommitAsync();
    }
}