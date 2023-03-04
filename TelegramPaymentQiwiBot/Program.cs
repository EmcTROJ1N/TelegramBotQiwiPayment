using System;
using System.Reflection;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;


namespace TelegramPaymentQiwiBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // channel to which the user will be redirected after payment
            long ChannelPrivateId = -100;
            PrivateChannelInviterOffer monthInviter = new PrivateChannelInviterOffer(1, 150, CurrencyEnum.Rub, "Приват на месяц 🔥",
                "Вы получите доступ к следующим ресурсам: \n -Nick`s Archives [ПРИВАТ] (канал)",
                new TimeSpan(31, 0, 0, 0), ChannelPrivateId);
            PrivateChannelInviterOffer alwaysInviter = new PrivateChannelInviterOffer(2, 250, CurrencyEnum.Rub,
                "Приват навсегда 🔥",
                "Вы получите доступ к следующим ресурсам: \n -Nick`s Archives [ПРИВАТ] (канал)",
                TimeSpan.MaxValue, ChannelPrivateId);

            List<BaseOffer> offers = new List<BaseOffer>() { monthInviter, alwaysInviter };
            TelegramBotPayment bot = new TelegramBotPayment(
                "",
                "",
                "",
                // The id of the channel into which the logs of purchases will be kept
                -100,
                offers
                );
            
            bot.StartHear();
            Console.Read();
        }
    }
}