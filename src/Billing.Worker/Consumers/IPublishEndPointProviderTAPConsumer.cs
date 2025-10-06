using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class IPublishEndPointProviderTAPConsumer(ILogger<IPublishEndPointProviderTAPConsumer> logger) : IConsumer<IPublishEndPointProviderRecord>
    {
        public async Task Consume(ConsumeContext<IPublishEndPointProviderRecord> context)
        {
            logger.LogInformation("Observed Message : Received message: {Message}", context.Message);
            await Task.CompletedTask;
        }
    }
}