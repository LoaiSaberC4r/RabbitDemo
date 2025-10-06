using Billing.Worker.Consumers;
using MassTransit;

namespace Billing.Worker.ConsumerDefinitions
{
    public sealed class IPublishEndPointProviderConsumerDefinition : ConsumerDefinition<IPublishEndPointProviderConsumer>
    {
        public IPublishEndPointProviderConsumerDefinition()
        {
            EndpointName = "ipublishendpointprovider-queue";
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<IPublishEndPointProviderConsumer> consumerConfigurator)
        {
            endpointConfigurator.PrefetchCount = 8;
            endpointConfigurator.UseMessageRetry(r => r.Immediate(3));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}