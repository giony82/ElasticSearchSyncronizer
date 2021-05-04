using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolUtils
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
         where TEntity : class
    {
        private readonly DbContext _dbContext;

        public GenericRepository(DbContext context) => _dbContext = context;

        private bool _inTransaction = false;

        public void BeginTransaction()
        {
            _inTransaction = true;
        }

        public async Task CommitTransactionAsync()
        {
            _inTransaction = false;

            await _dbContext.SaveChangesAsync();
        }

        public void RollBackTransaction()
        {
            _inTransaction = false;            
        }

        public async Task SaveChangesAsync()
        {
            if(!_inTransaction)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                return _dbContext.Set<TEntity>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)} entity must not be null!");
            }

            try
            {
                await _dbContext.AddAsync(entity);

                await SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)} entity must not be null!");
            }

            try
            {
                _dbContext.Remove(entity);

                await SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)} entity must not be null!");
            }

            try
            {
                _dbContext.Update(entity);

                await SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }        
    }
}
