// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotTest;

Console.WriteLine("Hello, World!");

const string token = "7125413978:AAEECZ-MQrdGWF8txh7EsLUr6jYo-u-73jw";

// if (token is null) throw new Exception("Bot client token not found");

var botClient = new TelegramBotClient(token);
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = new[]
    {
        UpdateType.Message
    }, // receive all update types except ChatMember related updates
};

var updateHandler = new TelegramUpdateHandler();

botClient.StartReceiving(
    updateHandler: updateHandler.HandleUpdateAsync,
    pollingErrorHandler: updateHandler.HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: default
);

Console.ReadLine();