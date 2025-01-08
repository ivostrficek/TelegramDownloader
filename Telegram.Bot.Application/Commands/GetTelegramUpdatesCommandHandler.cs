using FluentResults;
using Telegram.Bot.Application.ReturnObjects;

namespace Telegram.Bot.Application.Commands
{
    public class GetTelegramUpdatesCommandHandler : ICommandHandler<GetTelegramUpdatesCommand, IEnumerable<ReturnUpdate>>
    {
        private readonly ITelegramBotService _telegramBotService;
        public GetTelegramUpdatesCommandHandler(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task<Result<IEnumerable<ReturnUpdate>>> HandleAsync(GetTelegramUpdatesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _telegramBotService.GetUpdatesAsync(command.Offset, cancellationToken);
                return Result.Ok(result);
            }
            catch (Exception)
            {
                return Result.Fail<IEnumerable<ReturnUpdate>>("Error while getting updates");
            }
        }
    }
}
