using System.Text.RegularExpressions;
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
    ExpressionCalculator expressionCalculator = new ExpressionCalculator(msg);
    if (expressionCalculator.isValidExpression())    
    {
      reply += expressionCalculator.CalculateExpression().ToString();
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


public class ExpressionCalculator
{
  public string expression { get; private set; }

  public ExpressionCalculator(string exp)
  {
    expression = new string(exp);
  }
  public bool isValidExpression()
  {
    Regex regex = new Regex(@"([0-9]|\+|\-|\*|\/)*");
    return regex.IsMatch(expression);
  }
  public string CalculateExpression()
  {
    return Calculate(expression).ToString();
  }

  private decimal Calculate(string mathString)
  {
    //Removing whitespaces
    mathString = mathString.Replace(" ", "");

    //Check if there is brackets
    //match if there is couple of brackets with no brackets inside 
    Regex brackets = new Regex(@"\([^\(\)]*\)");

    //Find pair of brackets, calculate value inside and replace pair of
    //brackets by value inside it
    while (brackets.Match(mathString).Success)
    {
      Match target = brackets.Match(mathString);
      string bracketString = target.Value;
      string bracketCleanedString = bracketString.Replace("(", "");
      bracketCleanedString = bracketCleanedString.Replace(")", "");
      string calculated = Calculate(bracketCleanedString).ToString();
      mathString = mathString.Replace(bracketString, calculated);
    }

    char[] operators = { '+', '-', '*', '/' };
    //If contains operators
    foreach (char op in operators)
    {
      int operatorPosition = -1;
      decimal result = 0;
      if (mathString.Contains(op))
      {
        //calculate position of operator
        operatorPosition = mathString.IndexOf(op);
        switch (op)
        {
          case '+':
            return Calculate(mathString.Substring(0, operatorPosition))
                  + Calculate(mathString.Substring(operatorPosition + 1));

          case '-':
            return Calculate(mathString.Substring(0, operatorPosition))
                - Calculate(mathString.Substring(operatorPosition + 1));

          case '*':
            return Calculate(mathString.Substring(0, operatorPosition))
                * Calculate(mathString.Substring(operatorPosition + 1));

          case '/':
            return Calculate(mathString.Substring(operatorPosition + 1)) == 0 ? 0 :

                Calculate(mathString.Substring(0, operatorPosition))
                / Calculate(mathString.Substring(operatorPosition + 1));
        }
      }
      else
          if (Decimal.TryParse(mathString, out result))
        return result;
    }
    return 0;
  }
}
