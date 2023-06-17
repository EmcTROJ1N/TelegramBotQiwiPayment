using Qiwi.BillPayments.Model;
using DotNetEnv;
using System.Text.Json;

namespace TelegramPaymentQiwiBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Env.TraversePath().Load();

            string? qiwiSecretKey = Environment.GetEnvironmentVariable("QIWI_SECRET_KEY");
            string? qiwiOpenKey = Environment.GetEnvironmentVariable("QIWI_OPEN_KEY");
            string? telegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (qiwiSecretKey == null)
                throw new NullReferenceException("Qiwi secret key is null");
            if (qiwiOpenKey == null)
                throw new NullReferenceException("Qiwi open key is null");
            if (telegramBotToken == null)
                throw new NullReferenceException("Telegram bot token is null");

            if (args.Length == 0)
                throw new ArgumentException("Expected count of arguments: 1");

            long privateChannelId = -1111111111111;
            long reportChannelId = -1111111111111;

            PrivateChannelInviterOffer monthInviter = new PrivateChannelInviterOffer(1, 150, CurrencyEnum.Rub,
                "Test offer",
                "You will get access to offer 1 for a month",
                new TimeSpan(31, 0, 0, 0), privateChannelId);
            PrivateChannelInviterOffer alwaysInviter = new PrivateChannelInviterOffer(2, 250, CurrencyEnum.Rub,
                "Test offer 2",
                "You will get access to offer",
                TimeSpan.MaxValue, privateChannelId);

            List<BaseOffer> offers = new List<BaseOffer>() { monthInviter, alwaysInviter };

            TelegramBotPayment bot = new TelegramBotPayment(
                qiwiSecretKey,
                qiwiOpenKey,
                telegramBotToken,
                reportChannelId,
                offers
                );

            Task task = Task.Run(() => bot.StartHear());
            while (true) { Thread.Sleep(TimeSpan.FromDays(1)); }
        }
    }
}