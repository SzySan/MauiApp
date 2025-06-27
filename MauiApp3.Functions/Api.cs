using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MauiApp3.Functions.Services;

namespace MauiApp3.Functions
{
    public static class Api
    {
        // ręczna inicjalizacja serwisu
        private static readonly CloudService _svc = new CloudService();

        [FunctionName("GetProducts")]
        public static async Task<IActionResult> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req)
        {
            var list = await _svc.GetProductsAsync();
            return new OkObjectResult(list);
        }

        [FunctionName("AddOrder")]
        public static async Task<IActionResult> AddOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(body);

            await _svc.AddOrderAsync(order);

            return new OkResult();
        }

        private static readonly BlobService _blob = new BlobService();

        [FunctionName("ListProductImages")]
        public static async Task<IActionResult> ListImages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "images")] HttpRequest req)
        {
            var list = await _blob.ListBlobsAsync();
            return new OkObjectResult(list);
        }

        [FunctionName("UploadProductImage")]
        public static async Task<IActionResult> UploadImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "images/{name}")] HttpRequest req,
            string name)
        {
            var contentType = req.ContentType ?? "application/octet-stream";
            using var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            ms.Position = 0;
            var uri = await _blob.UploadBlobAsync(name, ms, contentType);
            return new OkObjectResult(new { Url = uri });
        }
    }
}
