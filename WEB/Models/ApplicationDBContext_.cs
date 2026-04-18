using Microsoft.EntityFrameworkCore;

namespace Website3.Web.Models
{
    public partial class ApplicationDbContext
    {
        public DbSet<Settings> Settings { get; set; }

        public void ConfigureModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(o => o.FullName)
                .HasComputedColumnSql("FirstName + ' ' + LastName");

        }

        public void AddNullableUniqueIndexes()
        {
        }
    }
}
