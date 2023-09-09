using OnlineBiddingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineBiddingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        internal readonly object Configuration;

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<NotificationViewModel> Notifications { get; set; }

    }
}
