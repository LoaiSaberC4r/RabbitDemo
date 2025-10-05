using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger, IPublishEndpoint publish) : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            var m = context.Message;
            logger.LogInformation("Processing Order {OrderId} for {Customer} Amount {Amount}",
         m.OrderId, m.CustomerId, m.Amount);

            if (m.Amount < 100)
            {
                logger.LogWarning("Rejecting Order {OrderId} (Amount too small)", m.OrderId);
                await publish.Publish(new OrderRejected(m.OrderId, m.CustomerId, m.Amount, "Amount too small", DateTime.UtcNow));
                var send = await context.GetSendEndpoint(new Uri("exchange:dlx.exchange?type=direct"));
                await send.Send(m, s => s.SetRoutingKey("orders.submitted.failed"));
                return;
            }
            await Task.Delay(100); // simulate processing
            await publish.Publish(new OrderAccepted(m.OrderId, m.CustomerId, m.Amount, DateTime.UtcNow));
            logger.LogInformation("Order {OrderId} accepted.", m.OrderId);
        }
    }
}