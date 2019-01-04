using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InfoScreenPi.Infrastructure
{
    public class InfoScreenContext : DbContext
    {
        public InfoScreenContext(DbContextOptions<InfoScreenContext> options) :base(options)
        { }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RssFeed> RssFeeds { get; set; }
        public DbSet<Background> Backgrounds { get; set; }
        public DbSet<ItemKind> ItemKinds { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }

            // User
            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(u => u.HashedPassword).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(u => u.Salt).IsRequired().HasMaxLength(200);

            // UserRole
            modelBuilder.Entity<UserRole>().Property(ur => ur.UserId).IsRequired();
            modelBuilder.Entity<UserRole>().Property(ur => ur.RoleId).IsRequired();

            // Role
            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired().HasMaxLength(50);

            //Inheritance
            /*modelBuilder.Entity<Item>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Item");
            });
            modelBuilder.Entity<User>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("User");
            });
            modelBuilder.Entity<RssFeed>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("RssFeed");
            });
            modelBuilder.Entity<Background>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Background");
            });
            modelBuilder.Entity<ItemKind>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("ItemKind");
            });
            modelBuilder.Entity<Role>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Role");
            });
            modelBuilder.Entity<UserRole>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("UserRole");
            });
            modelBuilder.Entity<Error>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Error");
            });
            modelBuilder.Entity<Setting>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Setting");
            });*/

            base.OnModelCreating(modelBuilder);
        }

    }

}
