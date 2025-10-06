using Billing.Worker.Consumers;
using MassTransit;

namespace Billing.Worker.ConsumerDefinitions
{
    public sealed class ISendEndpointProviderConsumerDefinition : ConsumerDefinition<ISendEndpointProviderConsumer>
    {
        public ISendEndpointProviderConsumerDefinition()
        {
            EndpointName = "isendendpointprovider-queue";
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ISendEndpointProviderConsumer> consumerConfigurator)
        {
            endpointConfigurator.PrefetchCount = 8;
            endpointConfigurator.UseMessageRetry(r => r.Immediate(3));
            endpointConfigurator.UseInMemoryOutbox();
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            {
                rmq.Bind("test-isend-endpoint-provider.exchange", x =>
                {
                    x.RoutingKey = "test-isend-endpoint-provider.key";
                    x.ExchangeType = "direct";
                });
                rmq.SetQueueArgument("x-dead-letter-exchange", "dlx.exchange");
                rmq.SetQueueArgument("x-dead-letter-routing-key", "orders.submitted.rejected");
            }
        }
    }
}