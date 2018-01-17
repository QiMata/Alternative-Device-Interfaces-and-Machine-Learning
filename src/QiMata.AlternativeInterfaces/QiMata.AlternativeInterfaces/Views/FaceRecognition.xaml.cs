using System;
using Plugin.Media;
using QiMata.AlternativeInterfaces.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QiMata.AlternativeInterfaces.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaceRecognition : ContentPage
    {
        private FaceService _faceService;

        public FaceRecognition()
        {
            InitializeComponent();
            _faceService = new FaceService("http://qimataalternativeinterface.azurewebsites.net/");
        }

        private async void UploadReleased(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            var person = await _faceService.DetectPerson(file.GetStream());

            await DisplayAlert("Person", person.Name, "OK");
        }

        private async void InitalizeReleased(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            await _faceService.CreateGroup(file.GetStream());

            await DisplayAlert("It worked", "Group created", "OK");
        }
    }
}