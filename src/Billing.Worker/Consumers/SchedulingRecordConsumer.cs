using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class SchedulingRecordConsumer(ILogger<SchedulingRecordConsumer> logger) : IConsumer<SchedulingRecord>
    {
        public async Task Consume(ConsumeContext<SchedulingRecord> context)
        {
            var msg = context.Message;
            logger.LogInformation("Received message Scheduling: {Message}", msg);
            await Task.CompletedTask;
        }
    }
}