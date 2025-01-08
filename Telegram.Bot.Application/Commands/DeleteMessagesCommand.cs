namespace Telegram.Bot.Application.Commands
{
    public class DeleteMessagesCommand
    {
        public long ChatId { get; set; }
        public List<int> MessageIdsToDelete { get; set; }
    }
}
