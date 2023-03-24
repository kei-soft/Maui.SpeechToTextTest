using System.Globalization;

namespace Maui.SpeechToTextTest;

public partial class MainPage : ContentPage
{
    private ISpeechToText speechToText;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public Command ListenCommand { get; set; }
    public Command ListenCancelCommand { get; set; }
    public Command SpeakCommand { get; set; }

    private string recognitionText;
    public string RecognitionText
    {
        get
        {
            return recognitionText;
        }

        set
        {
            recognitionText = value;
            OnPropertyChanged(nameof(RecognitionText));
        }
    }

    public MainPage(ISpeechToText speechToText)
    {
        InitializeComponent();

        this.speechToText = speechToText;

        ListenCommand = new Command(Listen);
        ListenCancelCommand = new Command(ListenCancel);
        SpeakCommand = new Command(Speak);

        BindingContext = this;
    }

    /// <summary>
    /// 듣고 결과 도출
    /// </summary>
    private async void Listen()
    {
        var isAuthorized = await speechToText.RequestPermissions();
        if (isAuthorized)
        {
            try
            {
                RecognitionText = await speechToText.Listen(CultureInfo.GetCultureInfo("en-us"),
                    new Progress<string>(partialText =>
                    {
                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            RecognitionText = partialText;
                        }
                        else
                        {
                            RecognitionText += partialText + " ";
                        }

                        OnPropertyChanged(nameof(RecognitionText));
                    }), tokenSource.Token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlert("Permission Error", "No microphone access", "OK");
        }
    }

    private void ListenCancel()
    {
        tokenSource?.Cancel();
    }


    CancellationTokenSource cts;

    /// <summary>
    /// 텍스트 말하기
    /// </summary>
    private async void Speak()
    {
        cts = new CancellationTokenSource();
        await TextToSpeech.Default.SpeakAsync(this.speakText.Text, cancelToken: cts.Token);
    }
}

