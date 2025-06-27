using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiApp3.Pages;  

namespace MauiApp3.Services
{
    public class ApiService
    {
        private readonly HttpClient _http = new()
        {
          
            BaseAddress = new Uri("https://mauiapp3-funcmauiapp3-func-gkc3byfng3axcnc5.westeurope-01.azurewebsites.net/api/")
        };

        // GET https://.../api/products
        public Task<List<Product>> GetProductsAsync() =>
            _http.GetFromJsonAsync<List<Product>>("products");

        // POST https://.../api/orders
        public async Task AddOrderAsync(Order order)
        {
            var response = await _http.PostAsJsonAsync("orders", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
