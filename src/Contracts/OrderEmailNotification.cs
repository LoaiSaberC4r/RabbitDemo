namespace Contracts
{
    public record OrderEmailNotification(Guid OrderId, string CustomerId, string Message, DateTime TimestampUtc);

}
