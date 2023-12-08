using Microsoft.EntityFrameworkCore;

namespace AppApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define the DbSet for your result class
        public DbSet<ProcedureResult> ProcedureResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map the stored procedure to the DbSet
            modelBuilder.Entity<ProcedureResult>()
                .HasNoKey()
                .ToView("YourStoredProcedureName");

            base.OnModelCreating(modelBuilder);
        }
    }

}
