using FluentResults;

namespace Telegram.Bot.Application
{
    public interface ICommandHandler<in T, TY>
    {
        Task<Result<TY>> HandleAsync(T command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<T>
    {
        Task<Result> HandleAsync(T command, CancellationToken cancellationToken);
    }
}
