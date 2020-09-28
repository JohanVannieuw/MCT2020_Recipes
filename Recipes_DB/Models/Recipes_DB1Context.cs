using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Recipes_DB.Data;

namespace Recipes_DB.Models
{
    public partial class Recipes_DB1Context : DbContext
    {
        public Recipes_DB1Context()
        {
        }

        public Recipes_DB1Context(DbContextOptions<Recipes_DB1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }

                
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS01;Initial Catalog=Recipes_DB1;Trusted_Connection=True;");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.CategoryName)
                    .IsUnique()
                    .HasFilter("([Category] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasColumnName("Category");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasIndex(e => e.CategoryId);

                entity.Property(e => e.Id)
                    .HasColumnName("RecipeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.RecipeName).IsRequired();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);

            modelBuilder.Seed();
           // ModelBuilder.Seed(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
