using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TgBotTest;

public class TelegramUpdateHandler : IUpdateHandler
{
  public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
  {
    // твой код
    string msg = update.Message.Text;
    string reply = "";
    MathExpression mathExpression = new MathExpression(msg);
    if (mathExpression.IsValidExpression())    
    {
      reply += mathExpression.Calculate().ToString();
    }
    
    await botClient.SendTextMessageAsync(update.Message.Chat.Id, reply);
  }

  public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
      CancellationToken cancellationToken)
  {
    var errorMessage = exception switch
    {
      ApiRequestException apiRequestException
          => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
      _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
  }
}
