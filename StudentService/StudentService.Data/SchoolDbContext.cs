using Microsoft.EntityFrameworkCore;
using StudentService.Data.Models;

namespace StudentService.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options):base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasOne<StudentProfile>(student => student.StudentProfile)
                .WithOne(profile => profile.Student).HasForeignKey<StudentProfile>(profile => profile.StudentId);
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }        
        public DbSet<StudentProfile> StudentProfiles { get; set; }        
    }
}
