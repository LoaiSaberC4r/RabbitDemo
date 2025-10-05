namespace Contracts
{
    public record OrderAccepted(Guid OrderId, string CustomerId, decimal Amount, DateTime TimestampUtc);

}
