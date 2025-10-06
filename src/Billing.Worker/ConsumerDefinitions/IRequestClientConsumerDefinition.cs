using Billing.Worker.Consumers;
using MassTransit;

namespace Billing.Worker.ConsumerDefinitions
{
    public sealed class IRequestClientConsumerDefinition : ConsumerDefinition<IRequestClientConsumer>
    {
        public IRequestClientConsumerDefinition()
        {
            EndpointName = "irequestclient-queue";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<IRequestClientConsumer> consumerConfigurator)
        {
            endpointConfigurator.PrefetchCount = 8;
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}