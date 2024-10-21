
using BidRestAPI.Model;
using Microsoft.EntityFrameworkCore;
namespace BidRestAPI.Data
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) // Passing the options to the base class constructor
        {
        }

        // Define your DbSets for the tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }
}