using System.Data.Entity;
using TaxiTicketSystem.Models;

namespace TaxiTicketSystem.Utils
{
    public class FinaContext : DbContext
    {
        public FinaContext() : base("FinaDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public DbSet<Cars> Cars { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Contragents> Contragents { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Users> Users { get; set; }

        public DbSet<GeneralDocs> GeneralDocs { get; set; }
        public DbSet<ProductOut> ProductOut { get; set; }
        public DbSet<ProductsFlow> ProductsFlow { get; set; }

        public DbSet<GroupContragents> GroupContragents { get; set; }
        public DbSet<Entries> Entries { get; set; }
        public DbSet<Params> Params { get; set; }
        public DbSet<Banks> Banks { get; set; }
        public DbSet<ContragentAccounts> ContragentAccounts { get; set; }
        public DbSet<Currencies> Currencies { get; set; }
        public DbSet<Cashes> Cashes { get; set; }
        public DbSet<CompanyAccounts> CompanyAccounts { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<Comments> Comments { get; set; }
    }
}