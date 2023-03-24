using System.Globalization;

namespace Maui.SpeechToTextTest
{
    public interface ISpeechToText
    {
        Task<bool> RequestPermissions();

        Task<string> Listen(CultureInfo cultureInfo, IProgress<string> result, CancellationToken cancellationToken);
    }
}
