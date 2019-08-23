using System;
using EXAMcsharp.Models;
using Microsoft.EntityFrameworkCore;

namespace EXAMcsharp.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}
       
        public DbSet <User> Users {get; set;}

        public DbSet <Actvty> Actvtys {get;set;}

        public DbSet <Membership> Memberships {get; set;}
       
    }
}