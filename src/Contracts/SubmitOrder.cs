namespace Contracts
{
    public record SubmitOrder(Guid OrderId, string CustomerId, decimal Amount, DateTime TimestampUtc);
}