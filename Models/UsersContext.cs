using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using RankDit.Models;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.Configuration;
namespace RankDit.DbContext
{
    public class UsersContext:Microsoft.Data.Entity.DbContext
    {
        public DbSet<User> Users{get;set;}
        public DbSet<Account>Accounts{get;set;}
        public DbSet<DeviceTokenEntity> Tokens { get; set; }
         public Microsoft.Framework.Configuration.IConfiguration Config { get; set; }
           protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().Key(e => e.UserID);
            builder.Entity<Account>().Key(e=>e.AccountID);
            builder.Entity<DeviceTokenEntity>().Key(p => p.TokenID);
            base.OnModelCreating(builder);
        }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
             var connString = //"Server=myinstance.cxy0h9vbivra.us-east-1.rds.amazonaws.com;Database=RankDit;User Id=sa;Password=sa12345678;";
            Startup.Configuration["Data:DefaultConnection:ConnectionString"];
             optionsBuilder.UseSqlServer(connString);
        }
        public UsersContext()
        {
        }
    }
}
