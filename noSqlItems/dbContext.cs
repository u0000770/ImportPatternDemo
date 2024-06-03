using MongoDB.Driver;

namespace noSqlItems
{
    using Domain;
    using Microsoft.Extensions.Configuration;
    using System.Configuration;

    public class MyMongoDbContext
    {
        private readonly IMongoCollection<Item> _itemsCollection;

        public MyMongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _itemsCollection = database.GetCollection<Item>("Items");
        }

        public MyMongoDbContext()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("MongoDbConnectionString");
            var databaseName = configuration["MongoDbDatabaseName"];
            var collectionName = configuration["MongoDbCollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _itemsCollection = database.GetCollection<Item>(collectionName);
        }

        public List<Item> GetAllItems()
        {
            // Query all items from the collection
            return _itemsCollection.Find(FilterDefinition<Item>.Empty).ToList();
        }

        public void SeedDatabase()
        {
            // Check if the collection already contains data
            if (_itemsCollection.CountDocuments(FilterDefinition<Item>.Empty) == 0)
            {
                // Create a new item
                var newItem = new Item
                {
                    Name = "Sample Item"
                };

                // Insert the item into the collection
                _itemsCollection.InsertOne(newItem);
            }
        }
    }
}
