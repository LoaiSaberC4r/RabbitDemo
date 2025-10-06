using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class IRequestClientConsumer(ILogger<IRequestClientConsumer> logger) : IConsumer<IRequestClientRecord>
    {
        public async Task Consume(ConsumeContext<IRequestClientRecord> context)
        {
            var msg = context.Message;
            logger.LogInformation("Received message: {Message}", msg);
            await context.RespondAsync(new IRequestClientRecord("okay received"));
        }
    }
}