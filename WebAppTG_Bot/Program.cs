using Telegram.Bot;
using Telegram.Bot.Polling;
using WebAppTG_Bot.OpenAI;
using WebAppTG_Bot.Configure;

var builder = WebApplication.CreateBuilder(args);

AppConfig.TGToken = builder.Configuration.GetSection("TGBot").GetSection("TGToken").Value;
AppConfig.BotId = builder.Configuration.GetSection("TGBot").GetSection("BotId").Value;
AppConfig.OpenAIToken = $"{builder.Configuration.GetSection("OpenAI").GetSection("OpenAITokenFirstPart").Value}" +
    $"{builder.Configuration.GetSection("OpenAI").GetSection("OpenAITokenSecondPart").Value}"; // for sucure
AppConfig.HTTPSApi = builder.Configuration.GetSection("OpenAI").GetSection("HTTPSApi").Value;

ITelegramBotClient bot = new TelegramBotClient(AppConfig.TGToken);

builder.Services.AddRazorPages();

builder.Services.AddSingleton(bot);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { },
};

bot.StartReceiving(
    Helper.HandleUpdateAsync,
    Helper.HandleErrorAsync,
    receiverOptions,
    cancellationToken
);
app.Run();