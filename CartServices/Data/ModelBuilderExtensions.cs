using CartServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Data
{
    public static class ModelBuilderExtensions
    {

        public static Microsoft.AspNetCore.Hosting.IWebHostEnvironment env { get; set; }

        public static Guid TESTUSERID = Guid.Parse("3fa85f64-5717-4562-b3fc-2c9631234567");
        public static CartServicesContext _context { get; set; }

        public static void Seed(this ModelBuilder modelBuilder)
        {
            Console.WriteLine("Seeding Cart and CartItems for testing");
            modelBuilder.Entity<Cart>().HasData(_carts);
            modelBuilder.Entity<CartItem>().HasData(_cartItems);
        }

        //Static Testdata
        private readonly static List<Cart> _carts = new List<Cart> {
            new Cart
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-1123456789aa"),
                UserId = TESTUSERID
            }
        };

        private readonly static List<CartItem> _cartItems = new List<CartItem> {
         new CartItem
           {
             Id = Guid.NewGuid(),
             ItemName="CartItem10_Test",
              ItemPrice =10.10M,
              Quantity =10,
             CartId = _carts[0].Id

           },
         new CartItem
         {
             Id = Guid.NewGuid(),
             ItemName="CartItem01_Test",
                       ItemPrice =25.5M,
                        Quantity =10,
                         CartId = _carts[0].Id
                    },
          new CartItem
                   {
             Id = Guid.NewGuid(),
                        ItemName="CartItem02_Test",
                        ItemPrice =10.0M,
                        Quantity =1,
                       CartId = _carts[0].Id
                    }
        };

        public static string UserId
        {
            get
            {
                if (env.EnvironmentName != "Production")
                {
                    return TESTUSERID.ToString();
                }
                else
                {
                    return null;
                }
            }
        }



    }
}