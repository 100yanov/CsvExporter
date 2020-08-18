using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace CsvExportService
{
    public class ShopDbContext : DbContext
    {
        private string connectionString;

        public DbSet<ItemEntity> Items { get; set; }
       
        public ShopDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(this.connectionString);             
        }        
    }
}
