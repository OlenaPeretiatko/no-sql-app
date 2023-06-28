using Microsoft.Azure.Cosmos;

namespace NoSQLApp.Services
{
    public class DbService
    {
        private readonly CosmosClient _client;
        private Database _database;
        private Container _container;
        private readonly string _databaseName;
        private readonly string _containerName;

        public DbService(string connectionString, string databaseName, string containerName)
        {
            _client = new CosmosClient(connectionString);
            _databaseName = databaseName;
            _containerName = containerName;
            _container = _client.GetContainer(databaseName, containerName);

            // Create the database and container if they do not exist
            InitializeDatabase().Wait();
            InitializeContainer().Wait();
        }

        private async Task InitializeDatabase()
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_databaseName);
            _database = databaseResponse.Database;
        }

        private async Task InitializeContainer()
        {
            var containerResponse = await _database.CreateContainerIfNotExistsAsync(_containerName, "/id");
            _container = containerResponse.Container;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var query = _container.GetItemQueryIterator<Product>();
            var results = new List<Product>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.Resource);
            }

            return results;
        }

        public async Task<Product> GetProductAsync(string itemId)
        {
            var response = await _container.ReadItemAsync<Product>(itemId, new PartitionKey(itemId));
            return response.Resource;
        }

        public async Task CreateProductAsync(Product newProduct)
        {
            await _container.CreateItemAsync<Product>(newProduct);
        }

        public async Task UpdateProductAsync(Product updatedProduct)
        {
            await _container.ReplaceItemAsync<Product>(updatedProduct, updatedProduct.Id);
        }

        public async Task DeleteProductAsync(string itemId)
        {
            await _container.DeleteItemAsync<Product>(itemId, new PartitionKey(itemId));
        }
    }
}
