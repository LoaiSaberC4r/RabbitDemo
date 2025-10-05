namespace Contracts
{
    public record OrderSmsNotification(Guid OrderId, string CustomerId, string Message, DateTime TimestampUtc);

}
