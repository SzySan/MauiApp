using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MauiApp3.Functions.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService()
        {
            var connStr = Environment.GetEnvironmentVariable("Blob__ConnectionString");
            var containerName = Environment.GetEnvironmentVariable("Blob__Container");
            _container = new BlobContainerClient(connStr, containerName);
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            var uris = new List<string>();
            await foreach (var blob in _container.GetBlobsAsync())
            {
                var client = _container.GetBlobClient(blob.Name);
                var sas = client.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
                uris.Add(sas.ToString());
            }
            return uris;
        }

        public async Task<string> UploadBlobAsync(string fileName, Stream data, string contentType)
        {
            var client = _container.GetBlobClient(fileName);
            await client.UploadAsync(data, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType });
            var sas = client.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
            return sas.ToString();
        }
    }
}
