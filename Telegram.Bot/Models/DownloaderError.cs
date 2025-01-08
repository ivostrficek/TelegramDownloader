using FluentResults;

namespace Telegram.Bot.Models
{
    public class DownloaderError : IError
    {
        public DownloaderError(string message, Exception? ex = null)
        {
            Message = message;
            Metadata = new Dictionary<string, object>() { { DownloaderErrorResponseType.Exception.ToString(), ex } };
        }

        public List<IError> Reasons => throw new NotImplementedException();

        public string Message { get; private set; }

        public Dictionary<string, object> Metadata { get; set; }
    }

    public enum DownloaderErrorResponseType
    {
        Exception
    }
}
