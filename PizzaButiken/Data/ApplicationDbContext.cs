using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Models;

namespace PizzaButiken.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DishIngredient>().HasKey(di => new { di.DishId, di.IngredientId });

            builder.Entity<DishIngredient>().HasOne(di => di.Dish).WithMany(d => d.DishIngredients).HasForeignKey(di => di.DishId);

            builder.Entity<DishIngredient>().HasOne(i => i.Ingredient).WithMany(d => d.DishIngredients).HasForeignKey(i => i.IngredientId);

            builder.Entity<Dish>().HasOne(dc => dc.DishCategory).WithMany(di => di.Dishes).HasForeignKey(dc => dc.DishCategoryId);

            builder.Entity<CartItemIngredient>().HasKey(ci => new { ci.CartItemId, ci.IngredientId });

            builder.Entity<CartItemIngredient>().HasOne(ci => ci.CartItem).WithMany(c => c.CartItmeIngredients).HasForeignKey(ci => ci.CartItemId);

            builder.Entity<CartItemIngredient>().HasOne(i => i.Ingredient).WithMany(ci => ci.CartItemIngredients).HasForeignKey(i => i.IngredientId);

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }
        public DbSet<DishCategory> DishCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemIngredient> CartItemIngredients { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
