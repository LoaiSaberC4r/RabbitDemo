using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class ISendEndpointProviderConsumer(ILogger<ISendEndpointProviderConsumer> logger) : IConsumer<ISendEndPointProviderRecord>
    {
        public async Task Consume(ConsumeContext<ISendEndPointProviderRecord> context)
        {
            var msg = context.Message;
            logger.LogInformation("Received message: {Message}", msg);
            await Task.CompletedTask;
        }
    }
}