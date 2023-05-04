using Telegram.Bot;
using Telegram.Bot.Types;
using FFMpegCore;
using TelegramBotExperiments;
using Telegram.Bot.Types.Enums;
using FFMpegCore.Enums;
using FFMpegCore.Arguments;

namespace TegelgamVoiceTranslator.modules
{
    public class CommandConverter : VoiceModelInicializator
    {
        /// <summary>
        /// Удаляет использованный файл чтоб не засорять память
        /// </summary>
        /// <param name="filePath"> путь к удаляемому файлу </param>
        private static void DestroyUsedAudioFiles(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка удаления: {0}", e.Message);
                }
            }
            else
            {
                Console.WriteLine("Указанный путь не существует!");
            }
        }

        /// <summary>
        /// Переводит файл из формата .ogg в .wav
        /// </summary>
        /// <param name="filePath"> путь к необходимому файлу </param>
        private static void TransformAoudioFileToWAV(string filePath)
        {
            FFMpegArguments
                .FromFileInput(filePath)
                .OutputToFile(filePath.Replace(".ogg", ".wav"), addArguments: options => options
            .WithAudioSamplingRate(128000).WithAudioBitrate(AudioQuality.Ultra))
                .ProcessSynchronously();
        }

        /// <summary>
        /// Активируется если бот получает текстовое сообщение
        /// </summary>
        /// <param name="botClient"> интерфейс клиента бота </param>
        /// <param name="receivedMessage"> полученное сообщение </param>
        /// <returns></returns>
        public static async Task OnTextGet(ITelegramBotClient botClient, Message receivedMessage)
        {
            if (receivedMessage.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(receivedMessage.Chat, "Дарова, этот бот короче расшифровфвает голосовые сообщения (по крайней мере пытается). \nМожешь даже проверить, мгм");
                return;
            }
        }

        /// <summary>
        /// Активируется если бот получает голосовое сообщение
        /// Вычленяет из него чистый текст
        /// </summary>
        /// <param name="botClient"> интерфейс клиента бота </param>
        /// <param name="receivedMessage"> полученное сообщение </param>
        /// <returns></returns>
        public static async Task OnVoiceGet(ITelegramBotClient botClient, Message receivedMessage)
        {
            var filePath = Path.Combine(Path.GetTempPath(), receivedMessage.Voice.FileId + ".ogg");

            using (var file = System.IO.File.OpenWrite(filePath))
            {
                var preparedFilePath = botClient.GetFileAsync(receivedMessage.Voice.FileId).Result.FilePath;
                if (preparedFilePath != null)
                {
                    await botClient.DownloadFileAsync(preparedFilePath, file);
                }
            }

            TransformAoudioFileToWAV(filePath);
            DestroyUsedAudioFiles(filePath);

            filePath = filePath.Replace(".ogg", ".wav");

            var model = voiceModel;

            // Получает сообщение в текстовом формате
            string textMessage = VoskDemo.DemoFloats(model, filePath);
            // Console.WriteLine("\n" + textMessage);

            DestroyUsedAudioFiles(filePath);

            await botClient.SendTextMessageAsync(receivedMessage.Chat, textMessage, replyToMessageId: receivedMessage.MessageId);
        }
        /// <summary>
        /// Перенаправляет полученное сообщение в зависимости от его типа
        /// </summary>
        /// <param name="botClient"> интерфейс клиента бота </param>
        /// <param name="receivedMessage"> полученное сообщение </param>
        /// <returns></returns>
        public static async Task OnMessageGet(ITelegramBotClient botClient, Message receivedMessage)
        {
            if (receivedMessage.Type.Equals(MessageType.Text))
            {
                if (!string.IsNullOrWhiteSpace(receivedMessage.Text)) await OnTextGet(botClient, receivedMessage);
            }
            else if (receivedMessage.Type.Equals(MessageType.Voice))
            {
                await OnVoiceGet(botClient, receivedMessage);
            }
            else
            {
                await botClient.SendTextMessageAsync(receivedMessage.Chat, "ERROR! Я пока н умею распозновать этот тип сообщений(");
            }

        }
    }
}
