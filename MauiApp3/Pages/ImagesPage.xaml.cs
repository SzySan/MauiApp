using Microsoft.Maui.Controls;
using MauiApp3.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace MauiApp3.Pages
{
    public partial class ImagesPage : ContentPage
    {
        private readonly BlobClient _blob = new BlobClient();

        // Lista URLi do obrazk�w
        public ObservableCollection<string> ImageUrls { get; set; }

        public ImagesPage()
        {
            InitializeComponent();

            ImageUrls = new ObservableCollection<string>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadImagesAsync();
        }

        async Task LoadImagesAsync()
        {
            try
            {
                var list = await _blob.GetImageUrlsAsync();
                ImageUrls.Clear();
                foreach (var url in list)
                    ImageUrls.Add(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("B��d", $"Nie uda�o si� pobra� obraz�w:\n{ex.Message}", "OK");
            }
        }

        async void OnUploadButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Wybierz obraz"
                });

                if (result == null)
                    return; // u�ytkownik anulowa�

                // odczytaj plik
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var data = ms.ToArray();

                // Wy�lij do backendu
                var url = await _blob.UploadImageAsync(
                    result.FileName,
                    data,
                    result.ContentType);

                // Dodaj do kolekcji, by od razu widzie�
                ImageUrls.Insert(0, url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("B��d", $"Upload nie powi�d� si�:\n{ex.Message}", "OK");
            }
        }
    }
}
