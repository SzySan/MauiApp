using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;   // ← dodane

namespace MauiApp3.Functions.Services
{
    public class CloudService
    {
        // Podmień na swoje wartości z Azure Portal:
        private const string EndpointUri = "https://mauiapp3.documents.azure.com:443/";
        private const string PrimaryKey = "??";
        private const string DatabaseId = "mauiapp3";
        private const string ProductsContainerId = "Products";
        private const string OrdersContainerId = "Orders";

        private readonly CosmosClient _client;
        private readonly Container _productsContainer;
        private readonly Container _ordersContainer;

        public CloudService()
        {
            _client = new CosmosClient(EndpointUri, PrimaryKey);

            // Upewnij się, że baza istnieje
            _client.CreateDatabaseIfNotExistsAsync(DatabaseId).GetAwaiter().GetResult();
            var db = _client.GetDatabase(DatabaseId);

            // Upewnij się, że kontenery istnieją
            db.CreateContainerIfNotExistsAsync(ProductsContainerId, "/Name").GetAwaiter().GetResult();
            db.CreateContainerIfNotExistsAsync(OrdersContainerId, "/Priority").GetAwaiter().GetResult();

            _productsContainer = db.GetContainer(ProductsContainerId);
            _ordersContainer = db.GetContainer(OrdersContainerId);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var query = _productsContainer.GetItemQueryIterator<Product>(
                              new QueryDefinition("SELECT * FROM c"));
            var results = new List<Product>();
            while (query.HasMoreResults)
            {
                var resp = await query.ReadNextAsync();
                results.AddRange(resp.Resource);
            }
            return results;
        }

        public async Task AddOrderAsync(Order order)
        {
            // Nadajemy id – Cosmos wymaga dokładnie pola "id"
            order.Id = Guid.NewGuid().ToString();
            try
            {
                await _ordersContainer.CreateItemAsync(order, new PartitionKey(order.Priority));
            }
            catch (CosmosException ex)
            {
                Debug.WriteLine($"Cosmos Error: {ex.StatusCode} – {ex.Message}");
                throw;
            }
        }
    }

    public class Product
    {
        [JsonProperty("id")]           // ← dzięki temu w JSON będzie "id"
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }
    }

    public class Order
    {
        [JsonProperty("id")]           // ← tutaj kluczowe
        public string Id { get; set; }

        [JsonProperty("ProductName")]
        public string ProductName { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonProperty("Priority")]
        public string Priority { get; set; }

        [JsonProperty("DueDate")]
        public DateTime DueDate { get; set; }
    }
}
