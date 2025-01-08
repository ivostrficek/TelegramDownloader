
// Set up a service collection
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Application;
using Telegram.Bot.Application.Commands;
using Telegram.Bot.Application.ReturnObjects;
using Telegram.Bot.Downloaders;
using Telegram.BotAPI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IDownloader, NewTikTokDownloader>()
    //.AddSingleton<IDownloader, PlayWrightDownloader>()
    .AddSingleton<App>()
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient(configuration["API_TOKEN"]))
    .AddSingleton<ITelegramBotService, TelegramBotService>()

    // Command Handlers
    .AddSingleton<ICommandHandler<GetTelegramUpdatesCommand, IEnumerable<ReturnUpdate>>, GetTelegramUpdatesCommandHandler>()
    .AddSingleton<ICommandHandler<SendMessageCommand>, SendMessageCommandHandler>()
    .AddSingleton<ICommandHandler<SendVideoCommand>, SendVideoCommandHandler>()
    .AddSingleton<ICommandHandler<DeleteMessagesCommand>, DeleteMessagesCommandHandler>()


    .BuildServiceProvider();

// Resolve the application and run it
var app = serviceProvider.GetService<App>();
await app.Run(new CancellationToken());
