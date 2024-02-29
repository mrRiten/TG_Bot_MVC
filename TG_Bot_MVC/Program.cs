﻿using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TG_Bot_MVC;

var veiw = new BotVeiw();
var controller = new ProgramController(veiw);

await RunBot(controller);

async Task RunBot(ProgramController controller)
{
    var _configWorker = new ConfigWorker();
    
    var bot = new TelegramBotClient(_configWorker.GetBotToken());

    veiw._bot = bot;

    using CancellationTokenSource cts = new();

    ReceiverOptions receiverOptions = new()
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    };

    bot.StartReceiving(
        updateHandler: controller.BotController,
        pollingErrorHandler: controller.ErrorBotController,
        receiverOptions: receiverOptions,
        cancellationToken: cts.Token
        );

    var me = await bot.GetMeAsync();

    Console.WriteLine($"Bot with name: @{me.Username} is runing");
    Console.ReadLine();

    // Send cancellation request to stop bot
    cts.Cancel();
}
