using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MauiApp3.Services
{
    public class BlobClient
    {
        private readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("https://mauiapp3-funcmauiapp3-func-gkc3byfng3axcnc5.westeurope-01.azurewebsites.net/api/")
        };

        public Task<List<string>> GetImageUrlsAsync()
        {
            return _http.GetFromJsonAsync<List<string>>("images");
        }

        public async Task<string> UploadImageAsync(string name, byte[] data, string contentType)
        {
            using var content = new ByteArrayContent(data);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            const string functionKey = "??";

            var target = $"images/{name}?code={Uri.EscapeDataString(functionKey)}";


            var resp = await _http.PostAsync($"images/{name}", content);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return result["Url"];
        }
    }
}
