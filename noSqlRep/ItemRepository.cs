namespace noSqlRep
{
    using Domain;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public interface IItemRepository
    {
        void Add(Item item);
        IEnumerable<Item> GetAll();
        // Other repository methods...
    }

    public class ItemRepository : IItemRepository
    {
        private readonly IMongoCollection<Item> _itemsCollection;


        public ItemRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("JustItems");
            _itemsCollection = database.GetCollection<Item>("Items");
            Clear();
        }


        public ItemRepository(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _itemsCollection = database.GetCollection<Item>("Items");
            Clear();
        }

        public void Clear()
        {
            var filter = Builders<Item>.Filter.Empty;
            _itemsCollection.DeleteMany(filter);
        }

        public void Add(Item item)
        {
            _itemsCollection.InsertOne(item);
        }

        public IEnumerable<Item> GetAll()
        {
            return _itemsCollection.Find(_ => true).ToList();
        }

        // Implement other repository methods...
    }
}
