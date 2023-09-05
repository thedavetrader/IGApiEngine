using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;
using System.Speech.Synthesis;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? WriteToFileCompleted;

        private class WriteToFileRequest
        {
            public string? Filename { get; set; }

            public string? Content { get; set; }

            public bool Append { get; set; } = false;
        }

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void WriteToFile()
        {
            try
            {
                var request = ApiRequestQueueItem.Parameters;

                if (request is not null)
                {
                    WriteToFileRequest writeToFileRequest = JsonConvert.DeserializeObject<WriteToFileRequest>(request);

                    if (writeToFileRequest is not null &&
                        !string.IsNullOrEmpty(writeToFileRequest.Filename) &&
                        !string.IsNullOrEmpty(writeToFileRequest.Content))
                    {
                        FileInfo fileInfo = new (writeToFileRequest.Filename);

                        if (fileInfo.Directory is not null)
                            fileInfo.Directory.Create();

                        Task.Run(async () => await File.WriteAllTextAsync(fileInfo.FullName, writeToFileRequest.Content)).Wait();

                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(WriteToFileCompleted);
            }
        }
    }
}