using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class TestRabbitMqConsumer(ILogger<TestRabbitMqConsumer> logger) : IConsumer<TestRabbitMq>
    {
        public Task Consume(ConsumeContext<TestRabbitMq> context)
        {
            logger.LogInformation("Received message: {Message}", context.Message);
            return Task.CompletedTask;
        }
    }
}