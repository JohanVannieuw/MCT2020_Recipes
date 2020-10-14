using System;
using System.Collections.Generic;
using System.Text;
using IdentityServices.Models;
//using Microsoft.AspNet.Identity.EntityFramework; //NOK moet de Core zijn.
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServices.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(entity =>
            {


                //Gezien de aangepaste tussentabellen(!) is het NOODZAKELIJK  om de PK/FK
                // instructies aan te maken voor SQL SERVER. Zoniet gebruikt/maakt het EF zijn eigen 
                // relaties aan en krijg je dubbel aangemaakte properties 
                // in deze tussentabellen (UserId1, RoleId1).
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId);

            });

        }


        }
}
