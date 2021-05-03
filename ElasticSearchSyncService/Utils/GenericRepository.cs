using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolUtils
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
         where TEntity : class
    {
        private readonly DbContext dbContext;

        public GenericRepository(DbContext context) => dbContext = context;

        private bool inTransaction = false;

        public void BeginTransaction()
        {
            inTransaction = true;
        }

        public async Task CommitTransactionAsync()
        {
            inTransaction = false;
            await this.dbContext.SaveChangesAsync();
        }

        public void RollBackTransaction()
        {
            inTransaction = false;            
        }

        public async Task SaveChangesAsync()
        {
            if(!this.inTransaction)
            {
                await this.dbContext.SaveChangesAsync();
            }
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                return this.dbContext.Set<TEntity>();
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
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await this.dbContext.AddAsync(entity);

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
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                this.dbContext.Remove(entity);

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
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                this.dbContext.Update(entity);

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
