using Billing.Worker.Consumers;
using MassTransit;

namespace Billing.Worker.ConsumerDefinitions
{
    public sealed class TopicConsumerDefinition : ConsumerDefinition<TopicConsumer>
    {
        public TopicConsumerDefinition()
        {
            EndpointName = "topic-consumer-endpoint";
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TopicConsumer> consumerConfigurator)
        {
            endpointConfigurator.PrefetchCount = 16;
            endpointConfigurator.UseInMemoryOutbox();
            endpointConfigurator.UseMessageRetry(r => r.Immediate(3));
            endpointConfigurator.ConfigureConsumeTopology = false;
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            {
                rmq.Bind("orders.topic", x =>
                {
                    x.RoutingKey = "order.*";
                    x.ExchangeType = "topic";
                });
            }
        }
    }
}