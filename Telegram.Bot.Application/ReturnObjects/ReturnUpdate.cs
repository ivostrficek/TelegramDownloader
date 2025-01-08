namespace Telegram.Bot.Application.ReturnObjects
{
    public class ReturnUpdate
    {
        public int UpdateId { get; set; }
        public ReturnMessage Message { get; set; }

        public class ReturnMessage
        {
            public int MessageId { get; set; }
            public ReturnChat Chat { get; set; }
            public string Text { get; set; }
            public long ChatId { get; set; }

            public class ReturnChat
            {
                public long Id { get; set; }
                public string Username { get; set; }
            }
        }
    }
}
