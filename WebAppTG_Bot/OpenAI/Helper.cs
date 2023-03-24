using Telegram.Bot.Types;
using Telegram.Bot;
using WebAppTG_Bot.Configure;

namespace WebAppTG_Bot.OpenAI
{
    public static class Helper
    {
        private const int tokens = 3500;
        private const string engine = "text-davinci-003";
        private const double temperature = 0.7;
        private const int topP = 1;
        private const int frequencyPenalty = 0;
        private const int presencePenalty = 0;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var question = update.Message.Text;
                if (string.IsNullOrEmpty(question)) return;

                var answer = OpenAI.callOpenAI(botClient, tokens, question, engine, temperature, topP, frequencyPenalty, presencePenalty).Normalize();

                if (!string.IsNullOrEmpty(answer))
                {
                    var chatId = update.Message.Chat.Id;
                    await botClient.SendTextMessageAsync(chatId, answer);
                }
            }
            catch (Exception e)
            {
                botClient.SendTextMessageAsync(AppConfig.BotId, e.Message);
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(AppConfig.BotId, exception.Message);
        }
    }
}
