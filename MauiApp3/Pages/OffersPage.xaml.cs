using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using MauiApp3.Services;   // ApiService i Product z Services

namespace MauiApp3.Pages
{
    public partial class OffersPage : ContentPage
    {
        private readonly ApiService _api = new ApiService();

        public ObservableCollection<Product> Offers { get; set; }
        public ObservableCollection<Product> FilteredOffers { get; set; }

        public OffersPage()
        {
            InitializeComponent();

            Offers = new ObservableCollection<Product>();
            FilteredOffers = new ObservableCollection<Product>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var products = await _api.GetProductsAsync();

                Offers.Clear();
                foreach (var p in products)
                    Offers.Add(p);

                FilteredOffers.Clear();
                foreach (var p in Offers)
                    FilteredOffers.Add(p);
            }
            catch (Exception ex)
            {
                await DisplayAlert("B³¹d", $"Nie uda³o siê pobraæ ofert:\n{ex.Message}", "OK");
            }
        }

        void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = e.NewTextValue?.ToLower() ?? string.Empty;
            FilteredOffers.Clear();

            foreach (var item in Offers.Where(p => p.Name.ToLower().Contains(filter)))
                FilteredOffers.Add(item);
        }

        async void OnOfferTapped(object sender, TappedEventArgs e)
        {
            var frame = (Frame)sender;
            await frame.ScaleTo(1.1, 100, Easing.CubicIn);
            await frame.ScaleTo(1, 100, Easing.CubicOut);

            var product = (Product)e.Parameter;
            await DisplayAlert("Wybrano produkt", product.Name, "OK");
        }
    }
}
