using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class TopicConsumer(ILogger<TopicConsumer> logger) : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            logger.LogInformation("TopicConsumer: Received SubmitOrder: {OrderId}, CustomerId: {CustomerId}", context.Message.OrderId, context.Message.CustomerId);
            return;
        }
    }
}