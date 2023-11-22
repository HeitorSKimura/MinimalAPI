using Microsoft.EntityFrameworkCore;

namespace MinimalAPI
{
    public class StudentDb : DbContext
    {
        public StudentDb(DbContextOptions<StudentDb> options)
            :base(options) { }
            
        public DbSet<Student> Students => Set<Student>();
    }
}
