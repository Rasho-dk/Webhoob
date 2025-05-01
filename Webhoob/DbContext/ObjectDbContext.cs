using Microsoft.EntityFrameworkCore;
using Webhoob.Models;

namespace Webhoob.DbContext
{
    public class ObjectDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ObjectDbContext(DbContextOptions<ObjectDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<WebhookSubscription> Webhooks { get; set; }


    }
}
