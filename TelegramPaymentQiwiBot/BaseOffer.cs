using Qiwi.BillPayments.Model;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TelegramPaymentQiwiBot
{
    [JsonConverter(typeof(OfferConverter))]
    abstract class BaseOffer
    {
        public enum OfferType
        {
            PrivateChannelInviteOffer
        }

        public OfferType Type { get; set; }
        public int Id { get; set; }
        public int Price { get; set; }
        public CurrencyEnum Currency { get; set; }
        public string OfferName { get; set; }
        public string Comment { get; set; }
        public TimeSpan Duration { get; set; }
        public abstract void PayConfirmed(params object?[] args);
        public abstract void TimeIsUp(params object?[] args);

        public BaseOffer(OfferType type, int id, int price, CurrencyEnum currency, string offerName, string comment, TimeSpan duration)
        {
            Type = type;
            Id = id;
            Price = price;
            Currency = currency;
            OfferName = offerName;
            Comment = comment;
            Duration = duration;
        }
    }
}