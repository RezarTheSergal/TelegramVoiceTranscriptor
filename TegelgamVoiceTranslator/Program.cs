using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using TegelgamVoiceTranslator.modules;
using Vosk;

namespace TelegramBotExperiments
{
    /// <summary>
    /// Класс инициализатор голосовой модели Vosk
    /// </summary>
    public class VoiceModelInicializator
    {
        public static Model voiceModel;
        public static void Inicialization() 
        {
            voiceModel = VoskDemo.InitializeVoiceModel();
        }
       
    }
    class Program
    {
        static readonly ITelegramBotClient bot = new TelegramBotClient("ENTER YOUR TOKEN HERE");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update, Newtonsoft.Json.Formatting.Indented).Normalize());
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message != null)
                {
                    await CommandConverter.OnMessageGet(botClient, message);
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
             await Console.Out.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(exception, Newtonsoft.Json.Formatting.Indented).Normalize());
            await Console.Out.WriteLineAsync("\n"+exception);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Инициализация голосовой модели");

            VoiceModelInicializator inicializator = new VoiceModelInicializator();
            VoiceModelInicializator.Inicialization();

            Console.WriteLine("Готово!");
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // получает все виды событий
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}