using Contracts;
using MassTransit;

namespace Billing.Worker.Consumers
{
    public sealed class SmsNotificationConsumer(ILogger<SmsNotificationConsumer> logger)
    : IConsumer<OrderSmsNotification>
    {
        public Task Consume(ConsumeContext<OrderSmsNotification> ctx)
        {
            logger.LogInformation("[SMS] Order {OrderId} -> {Msg}", ctx.Message.OrderId, ctx.Message.Message);
            return Task.CompletedTask;
        }
    }
}