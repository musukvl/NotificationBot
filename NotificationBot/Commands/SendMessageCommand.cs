using System;
using Amba.ImageTools.Infrastructure;
using Microsoft.Extensions.CommandLineUtils;
using NotificationBot.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;
using NotificationBot.Configuration;

namespace NotificationBot.Commands
{
    public class SendMessageCommand : ICommand
    {
        private readonly TelegramBotClient _bot;

        public SendMessageCommand(TelegramBotClientSettings telegramBotClientSettings)
        {
            _bot = new TelegramBotClient(telegramBotClientSettings.ApiKey);
        }

        public string Name
        {
            get { return "send"; }
        }        

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Sends message to chat";
            var messageOption = command.Option("-m | --m | --message", "Message", CommandOptionType.SingleValue);
            var attachOption = command.Option("-a | --a | --attach", "Attached file", CommandOptionType.MultipleValue);
            var idArgument = command.Argument("chatId", "Recipient chat id");

            command.HelpOption("-? | -h | --h | --help");
            command.OnExecute(async () =>
            {
                var message = messageOption.GetValue();
                var id = idArgument.GetValue<long>(0);
                if (string.IsNullOrEmpty(message) || id == 0)
                {
                    command.ShowHelp(Name);
                    return 1;
                }

                await _bot.SendTextMessageAsync(id, message);

                if (attachOption.HasValue())
                {
                    foreach (var value in attachOption.Values)
                    {
                        try
                        {
                            using (var stream = System.IO.File.OpenRead(value))
                            {
                                await _bot.SendDocumentAsync(id, new FileToSend(Path.GetFileName(value), stream));
                            }
                        }
                        catch { }
                    }
                }
                
                return 0;
            });
        }      
    }
}
