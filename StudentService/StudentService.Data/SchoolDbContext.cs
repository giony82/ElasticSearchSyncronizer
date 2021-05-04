using Microsoft.EntityFrameworkCore;
using StudentService.Data.Models;

namespace StudentService.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options):base(options)
        {
                
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }        
    }
}
