using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class EmailNotificationConsumer(ILogger<EmailNotificationConsumer> logger)
     : IConsumer<OrderEmailNotification>
    {
        public Task Consume(ConsumeContext<OrderEmailNotification> ctx)
        {
            logger.LogInformation("[EMAIL] Order {OrderId} -> {Msg}", ctx.Message.OrderId, ctx.Message.Message);
            return Task.CompletedTask;
        }
    }
}