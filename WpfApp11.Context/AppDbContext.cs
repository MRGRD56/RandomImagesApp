using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WpfApp11.Model.DbModels;

namespace WpfApp11.Context
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        private const string ConnectionString = @"data source=localhost\SQLEXPRESS;initial catalog=SchoolTestDb;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        public AppDbContext() : base(ConnectionString)
        {

        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
