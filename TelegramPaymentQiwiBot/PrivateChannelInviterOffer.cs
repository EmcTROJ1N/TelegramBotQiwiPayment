using System.Text.Json.Serialization;
using Qiwi.BillPayments.Model;
using Telegram.Bot;

namespace TelegramPaymentQiwiBot
{
    class PrivateChannelInviterOffer : BaseOffer
    {
        public long? Channel { get; set; }
        
        [JsonConstructor]
        public PrivateChannelInviterOffer() {}
        public PrivateChannelInviterOffer(int id, int price, CurrencyEnum currency, string offerName, string comment, TimeSpan until, long channel)
        {
            Id = id;
            Price = price;
            Currency = currency;
            OfferName = offerName;
            Comment = comment;
            Duration = until;
            Channel = channel;
        }

        public async override void PayConfirmed(params object?[] args)
        {
            if (Channel == null) throw new NullReferenceException("channel is null");
            TelegramBotClient? bot = args[0] as TelegramBotClient;
            if (bot == null) throw new NullReferenceException("incorrect arg");
            long userId = (long)args[1];

            try
            {
                await bot.UnbanChatMemberAsync(Channel, userId);
            } catch {}
            try
            {
                await bot.ApproveChatJoinRequest(Channel, userId);
            } catch {}
        }

        public async override void TimeIsUp(object?[] args)
        {
            if (Channel == null) throw new NullReferenceException("channel is null");
            TelegramBotClient? bot = args[0] as TelegramBotClient;
            if (bot == null) throw new NullReferenceException("incorrect arg");
            long userId = (long)args[1];

            try
            {
                await bot.KickChatMemberAsync(Channel, userId);
                await bot.UnbanChatMemberAsync(Channel, userId);
                await bot.SendTextMessageAsync(userId, $"Срок услуги {OfferName} истек, сожалеем");
            }
            catch {}
        }
    }
}