using Qiwi.BillPayments.Model;

namespace TelegramPaymentQiwiBot
{
    abstract class BaseOffer
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public CurrencyEnum Currency { get; set; }
        public string OfferName { get; set; }
        public string Comment { get; set; }
        public TimeSpan Duration { get; set; }
        public abstract void PayConfirmed(params object?[] args);
        public abstract void TimeIsUp(params object?[] args);
    }
}