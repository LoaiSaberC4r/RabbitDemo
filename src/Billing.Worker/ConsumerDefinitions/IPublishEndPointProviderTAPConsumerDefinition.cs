using Billing.Worker.Consumers;
using Contracts;
using MassTransit;

namespace Billing.Worker.ConsumerDefinitions
{
    public sealed class IPublishEndPointProviderTAPConsumerDefinition : ConsumerDefinition<IPublishEndPointProviderTAPConsumer>
    {
        public IPublishEndPointProviderTAPConsumerDefinition()
        {
            EndpointName = "order-events-tap";
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<IPublishEndPointProviderTAPConsumer> consumerConfigurator)
        {
            endpointConfigurator.PrefetchCount = 16;
            endpointConfigurator.ConfigureConsumeTopology = false;
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            {
                rmq.Bind<IPublishEndPointProviderRecord>();
            }
        }
    }
}