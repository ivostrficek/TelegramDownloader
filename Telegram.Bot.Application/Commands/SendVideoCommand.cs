namespace Telegram.Bot.Application.Commands
{
    public class SendVideoCommand
    {
        public string Caption { get; set; }
        public string VideoPath { get; set; }
        public long ChatId { get; set; }
    }
}
