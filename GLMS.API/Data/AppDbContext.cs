using Microsoft.EntityFrameworkCore;
using GLMS.API.Models; // This tells the file where to look for your Client, Contract, and ServiceRequest classes

namespace GLMS.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}