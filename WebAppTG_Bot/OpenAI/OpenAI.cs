using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using Telegram.Bot;
using WebAppTG_Bot.Configure;

namespace WebAppTG_Bot.OpenAI
{
    internal static class OpenAI
    {
        public static string callOpenAI(ITelegramBotClient botClient, int tokens, string input, string engine,
            double temperature, int topP, int frequencyPenalty, int presencePenalty)
        {
            var openAiKey = AppConfig.OpenAIToken;
            var apiCall = AppConfig.HTTPSApi + engine + "/completions";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                        request.Content = new StringContent("{\n  \"prompt\": \"" + input + "\",\n  \"temperature\": " +
                                                            temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + tokens + ",\n  \"top_p\": " + topP +
                                                            ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");

                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = httpClient.SendAsync(request).Result;
                        var json = response.Content.ReadAsStringAsync().Result;

                        dynamic dynObj = JsonConvert.DeserializeObject(json);

                        if (dynObj != null)
                        {
                            return dynObj.choices[0].text.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                botClient.SendTextMessageAsync(AppConfig.BotId, e.Message);
            }
            return null;
        }
    }
}
