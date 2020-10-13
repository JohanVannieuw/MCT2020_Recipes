using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CartServices.Models;

namespace CartServices.Data
{
    public class CartServicesContext : DbContext
    {
        public CartServicesContext (DbContextOptions<CartServicesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            //base.OnModelCreating(modelBuilder);
         }
    }
}
