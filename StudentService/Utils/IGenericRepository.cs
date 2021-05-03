using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolUtils
{
    public interface IGenericRepository<TEntity>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task CommitTransactionAsync();
        Task<TEntity> DeleteAsync(TEntity entity);        
        void RollBackTransaction();
        Task SaveChangesAsync();
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}
