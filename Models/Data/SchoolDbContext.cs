using Microsoft.EntityFrameworkCore;
using SchoolManager.Models;

namespace SchoolManager.Web.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<School> Schools { get; set; }
    }
}
