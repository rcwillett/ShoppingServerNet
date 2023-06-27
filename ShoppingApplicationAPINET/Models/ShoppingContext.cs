using System;
using Microsoft.EntityFrameworkCore;

namespace ShoppingApplicationAPINET.Models
{
	public class ShoppingContext : DbContext
	{

        public ShoppingContext(DbContextOptions<ShoppingContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<User>()
				.HasMany(u => u.ShoppingItems)
				.WithOne(s => s.User)
				.HasForeignKey(s => s.User_ID);
        }

        public DbSet<ShoppingItem> ShoppingItems { get; set; }

		public DbSet<User> Users { get; set; }
	}
}

