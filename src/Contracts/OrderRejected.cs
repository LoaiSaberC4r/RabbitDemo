namespace Contracts
{
    public record OrderRejected(Guid OrderId, string CustomerId, decimal Amount, string Reason, DateTime TimestampUtc);

}
