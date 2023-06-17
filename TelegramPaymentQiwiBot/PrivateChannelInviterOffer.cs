using System.Text.Json.Serialization;
using Qiwi.BillPayments.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramPaymentQiwiBot
{
    class PrivateChannelInviterOffer : BaseOffer
    {
        public long? Channel { get; set; }
        public ChatInviteLink? ChannelInviteLink { get; set; }
        
        public PrivateChannelInviterOffer(int id, int price, CurrencyEnum currency, string offerName, string comment, TimeSpan duration, long channel)
            : base(OfferType.PrivateChannelInviteOffer, id, price, currency, offerName, comment, duration)
        {
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
                // await bot.KickChatMemberAsync(Channel, userId);
                await bot.BanChatMemberAsync(Channel, userId);
                await bot.SendTextMessageAsync(userId, $"Срок услуги {OfferName} истек, сожалеем");
            }
            catch {}
        }
    }
}