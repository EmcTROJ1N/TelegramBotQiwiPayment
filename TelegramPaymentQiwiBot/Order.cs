namespace TelegramPaymentQiwiBot;

class Order
{
    public long UserId { get; set; }
    public int OfferId { get; set; }
    public DateTime Until { get; set; }

    public Order(long userId, int offerId, DateTime until)
    {
        UserId = userId;
        OfferId = offerId;
        Until = until;
    }
}