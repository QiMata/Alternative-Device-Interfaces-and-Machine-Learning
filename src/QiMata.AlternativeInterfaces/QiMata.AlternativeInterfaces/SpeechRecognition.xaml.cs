using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Plugin.AudioRecorder;
//using Plugin.MediaManager;
using QiMata.AlternativeInterfaces.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QiMata.AlternativeInterfaces
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeechRecognition : ContentPage
    {
        private const int SixTeenThousand = 16000;

        private readonly AudioRecorderService _audioRecorderService;
        private readonly SpeakerRecognitionService _identificationServiceClient;

        private Task<string> _audioTask;

        public SpeechRecognition()
        {
            InitializeComponent();

            _audioRecorderService = new AudioRecorderService
            {
                StopRecordingOnSilence = false, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
                TotalAudioTimeout = TimeSpan.FromSeconds(15), //audio will stop recording after 15 seconds
                PreferredSampleRate = SixTeenThousand
            };

            _identificationServiceClient = new SpeakerRecognitionService("http://qimataalternativeinterface.azurewebsites.net/");
        }

        private async void Recognize_Button_OnReleased(object sender, EventArgs e)
        {
            if (!_audioRecorderService.IsRecording)
            {
                _audioRecorderService.TotalAudioTimeout = TimeSpan.FromSeconds(5);
                _audioTask = await _audioRecorderService.StartRecording();
                IsRecordingLabel.Text = "Is Recording";
            }

            var audioFile = await _audioTask;

            if (audioFile == null)
            {
                return;
            }
            IsRecordingLabel.Text = "Not Recording";

            var audioStream = _audioRecorderService.GetAudioFileStream();

            var profiles =
                await _identificationServiceClient.GetAllProfilesAsync();
            var ids = profiles.Select(x => x.ProfileId).ToArray();
            var id = await _identificationServiceClient.IdentifyAsync(audioStream, ids,true);

            var identificationResult = await _identificationServiceClient.CheckIdentificationStatusAsync(id);

            while (identificationResult.Status == Status.Running || identificationResult.Status == Status.NotStarted)
            {
                identificationResult = await _identificationServiceClient.CheckIdentificationStatusAsync(id);
            }

            if (identificationResult.Status == Status.Failed)
            {
                await DisplayAlert("Failed", $"Matching Failed: {identificationResult.Message}", "Ok");
                return;
            }

            IsRecordingLabel.Text = $"I am {identificationResult.ProcessingResult.Confidence} confident that its you";
        }

        private async void Create_Profile_Button_OnReleased(object sender, EventArgs e)
        {
            var profiles = await _identificationServiceClient.GetAllProfilesAsync();

            if (profiles.Length < 1)
            {
                await _identificationServiceClient.CreateProfileAsync("en-US");
                profiles = await _identificationServiceClient.GetAllProfilesAsync();
            }

            if (profiles[0].RemainingEnrollmentSpeechSeconds <= 0)
            {
                await DisplayAlert("Enrolled", "Your have fully enrolled", "Ok");
                return;
            }

            if (!_audioRecorderService.IsRecording)
            {
                _audioRecorderService.TotalAudioTimeout = TimeSpan.FromSeconds(15);
                _audioTask = await _audioRecorderService.StartRecording();
                IsRecordingLabel.Text = "Is Recording";
            }
            await Task.Delay(TimeSpan.FromSeconds(15));
            await _audioRecorderService.StopRecording();
            var audioFile = await _audioTask;
            
            if (audioFile == null)
            {
                return;
            }
            IsRecordingLabel.Text = "Not Recording";

            var audioStream = _audioRecorderService.GetAudioFileStream();
            //await CrossMediaManager.Current.Play(_audioRecorderService.GetAudioFilePath());
            var enrollment = await _identificationServiceClient.EnrollAsync(audioStream, profiles[0].ProfileId);

            var enrollmentOperation = await _identificationServiceClient.CheckEnrollmentStatusAsync(enrollment);
            while (enrollmentOperation.Status == Status.Running || enrollmentOperation.Status == Status.NotStarted)
            {
                enrollmentOperation = await _identificationServiceClient.CheckEnrollmentStatusAsync(enrollment);
            }

            if (enrollmentOperation.ProcessingResult.EnrollmentStatus != EnrollmentStatus.Enrolled)
            {
                await DisplayAlert("Not Enrolled", "You are not fully Enrolled", "Ok");
            }
        }
    }
}