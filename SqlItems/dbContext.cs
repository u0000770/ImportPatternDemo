namespace SqlItems
{
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.IO;
    using System.Linq;

    public class MyDbContext : DbContext
    {

        public DbSet<Item> Items { get; set; }

        private string _connectionString;

        public MyDbContext(){}

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        // Overloaded constructor accepting a connection string
        public MyDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // If a connection string is provided, use it; otherwise, use a default connection string
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
            else
            {
                // Get the path to the "Documents" folder
                string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Combine the "Documents" folder path with the default database filename
                string databasePath = Path.Combine(documentsFolder, "Items.db");

                // Configure the database provider and connection string for SQLite
                optionsBuilder.UseSqlite($"Data Source={databasePath}");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Items");
        }
    

        public void SeedDatabase()
        {
            // Check if the collection already contains data
            if (!Items.Any())
            {
                // Create a new item
                var newItem = new Item
                {
                    Name = "Barry Sql Hebbron"
                };

                // Add the item to the collection
                Items.Add(newItem);

                // Save changes to the database
                SaveChanges();
            }
        }
    }
}

