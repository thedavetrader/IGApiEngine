using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;
using System.Speech.Synthesis;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? SpeakCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void Speak()
        {
            try
            {
                var request = ApiRequestQueueItem.Parameters;

                if (!string.IsNullOrEmpty(request))
                {
                    if (OperatingSystem.IsWindows())
                    {
                        using SpeechSynthesizer synth = new();

                        synth.Rate = 3; // TODO: Make configurable synth.Rate
                        synth.SelectVoiceByHints(VoiceGender.Male);

                        Task.Factory.StartNew(() =>
                        {
                            WriteInformational(Columns(request));

                            if (OperatingSystem.IsWindows())
                                synth.Speak(request);
                            else
                                throw new PlatformNotSupportedException(nameof(Speak));
                        }).Wait();
                    }
                    else throw new PlatformNotSupportedException(nameof(Speak));
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(SpeakCompleted);
            }
        }
    }
}