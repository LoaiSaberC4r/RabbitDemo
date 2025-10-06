using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class IPublishEndPointProviderConsumer(ILogger<IPublishEndPointProviderConsumer> logger) : IConsumer<IPublishEndPointProviderRecord>
    {
        public async Task Consume(ConsumeContext<IPublishEndPointProviderRecord> context)
        {
            var msg = context.Message;
            logger.LogInformation("Received message: {Message}", msg);
            await Task.CompletedTask;
        }
    }
}