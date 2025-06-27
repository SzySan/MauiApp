using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using MauiApp3.Services; // <-- dostosuj namespace jeśli trzeba

namespace MauiApp3.Services
{
    public static class FileService
    {
        // Ścieżka do pliku w katalogu aplikacji
        private static string OrdersFilePath =>
            Path.Combine(FileSystem.AppDataDirectory, "orders.txt");

        // Dopisuje jedno zamówienie do końca pliku
        public static async Task AppendOrderAsync(Order order)
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm}: " +
                       $"{order.Quantity}×{order.ProductName} " +
                       $"(Priority: {order.Priority}, Due: {order.DueDate:yyyy-MM-dd})" +
                       Environment.NewLine;

            // Używamy File.AppendAllText synchronously – dla małych plików OK
            File.AppendAllText(OrdersFilePath, line);
            await Task.CompletedTask;
        }

        // Odczytuje całą zawartość pliku
        public static string ReadOrders()
        {
            if (!File.Exists(OrdersFilePath))
                return string.Empty;

            return File.ReadAllText(OrdersFilePath);
        }

        // Wywołuje dialog systemowy do udostępnienia pliku
        public static async Task ShareOrdersAsync()
        {
            if (!File.Exists(OrdersFilePath))
                return;

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Historia zamówień",
                File = new ShareFile(OrdersFilePath)
            });
        }
    }
}