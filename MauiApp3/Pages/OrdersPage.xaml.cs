using System;
using MauiApp3.Services;          // ApiService i FileService
using Microsoft.Maui.Controls;    // Button, ContentPage, Easing

namespace MauiApp3.Pages
{
    public partial class OrdersPage : ContentPage
    {
        // 1) Tu trzymamy instancjê ApiService zamiast CloudService
        private readonly ApiService _api = new ApiService();

        public OrdersPage()
        {
            InitializeComponent();
            priorityPicker.SelectedIndex = 0;
        }

        async void OnOrderButtonClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            // animacja przycisku
            await btn.ScaleTo(0.9, 50, Easing.Linear);
            await btn.ScaleTo(1, 50, Easing.Linear);

            // walidacja pól
            if (string.IsNullOrWhiteSpace(productEntry.Text) ||
                string.IsNullOrWhiteSpace(quantityEntry.Text))
            {
                await DisplayAlert("B³¹d", "Wype³nij wszystkie pola", "OK");
                return;
            }

            var order = new Order
            {
                ProductName = productEntry.Text,
                Quantity = int.Parse(quantityEntry.Text),
                Priority = (string)priorityPicker.SelectedItem,
                DueDate = datePicker.Date
            };

            try
            {
                // 2) Wyœlij do hostowanego backendu
                await _api.AddOrderAsync(order);

                // 3) Dopisz do lokalnego pliku
                await FileService.AppendOrderAsync(order);

                // 4) Sukces
                await DisplayAlert(
                    "Sukces",
                    $"Zamówienie wys³ane: {order.Quantity}×{order.ProductName}",
                    "OK");

                // 5) Wyczyœæ formularz
                productEntry.Text = string.Empty;
                quantityEntry.Text = string.Empty;
                priorityPicker.SelectedIndex = 0;
                datePicker.Date = DateTime.Today;
            }
            catch (Exception ex)
            {
                // w razie b³êdu (np. sieci) poka¿ komunikat
                await DisplayAlert(
                    "B³¹d",
                    $"Nie uda³o siê wys³aæ zamówienia:\n{ex.Message}",
                    "OK");
            }
        }

        async void OnShowHistoryClicked(object sender, EventArgs e)
        {
            var content = FileService.ReadOrders();
            if (string.IsNullOrWhiteSpace(content))
                await DisplayAlert("Historia", "Brak zapisanych zamówieñ.", "OK");
            else
                await DisplayAlert("Historia zamówieñ", content, "OK");
        }

        async void OnShareHistoryClicked(object sender, EventArgs e)
        {
            await FileService.ShareOrdersAsync();
        }
    }
}
