using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Website3.Web.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Error> Errors { get; set; }
        public DbSet<ErrorException> Exceptions { get; set; }

        private Settings _settings;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public Settings GetDbSettings()
        {
            return _settings ??= Settings.Single();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(false);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModelBuilder(modelBuilder);

            modelBuilder.Entity<User>(o => o.HasMany(u => u.Roles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // set all global query filters here - use IsInRole if needed, roles retrieved using httpContextAccessor...
            //modelBuilder.Entity<XXX>(xxx => xxx.HasQueryFilter(o => identityService.GetXXX() == null || o.Xxx == identityService.GetXXX()));
        }

        private void CreateNullableUniqueIndex(string tableName, string fieldName)
        {
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
            Database.ExecuteSqlRaw($"DROP INDEX IF EXISTS IX_{tableName}_{fieldName} ON {tableName};");
            Database.ExecuteSqlRaw($"CREATE UNIQUE NONCLUSTERED INDEX IX_{tableName}_{fieldName} ON {tableName}({fieldName}) WHERE {fieldName} IS NOT NULL;");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
        }

    }
}