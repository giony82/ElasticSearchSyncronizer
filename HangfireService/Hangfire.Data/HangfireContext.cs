
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Data
{
    /// <summary>
    /// Hangfire does not create automatically the database. For our purpose will create it but is not somethin
    /// </summary>
    public sealed class HangfireContext : DbContext
    {
        public HangfireContext(DbContextOptions<HangfireContext> options):base(options)
        {
            
        }

        public void Create()
        {
            this.Database.EnsureCreated();
        }
    }
}
