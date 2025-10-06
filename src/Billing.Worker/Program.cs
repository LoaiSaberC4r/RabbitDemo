using Billing.Worker.ConsumerDefinitions;
using Billing.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // 1) Consumers
    x.AddConsumer<SubmitOrderConsumer>(cfg => cfg.UseConcurrentMessageLimit(8));
    x.AddConsumer<SmsNotificationConsumer>();
    x.AddConsumer<EmailNotificationConsumer>();
    x.AddConsumer<TestRabbitMqConsumer>();
    x.AddConsumer<ISendEndpointProviderConsumer, ISendEndpointProviderConsumerDefinition>();
    x.AddConsumer<IPublishEndPointProviderConsumer, IPublishEndPointProviderConsumerDefinition>();
    x.AddConsumer<IRequestClientConsumer, IRequestClientConsumerDefinition>();
    x.AddConsumer<IPublishEndPointProviderTAPConsumer, IPublishEndPointProviderTAPConsumerDefinition>();

    // 2) Endpoints

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "dev", h =>
        {
            h.Username("admin");
            h.Password("admin_ChangeMe!");
        });

        cfg.PrefetchCount = 16;

        // ===== Direct: orders.exchange -> orders-submitted-queue =====
        cfg.ReceiveEndpoint("orders-submitted-queue", e =>
        {
            e.Bind("orders.exchange", x =>
            {
                x.RoutingKey = "order.submitted";
                x.ExchangeType = "direct";
            });

            // Reliability
            e.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            e.UseInMemoryOutbox();

            // DLX
            e.SetQueueArgument("x-dead-letter-exchange", "dlx.exchange");
            e.SetQueueArgument("x-dead-letter-routing-key", "orders.submitted.failed");

            // e.SetQueueArgument("x-max-length", 1000);
            // e.SetQueueArgument("x-overflow", "drop-head");

            e.ConfigureConsumer<SubmitOrderConsumer>(ctx);
        });

        // DLQ endpoint
        cfg.ReceiveEndpoint("orders-submitted-queue.dlq", e =>
        {
            e.Bind("dlx.exchange", x =>
            {
                x.RoutingKey = "orders.submitted.failed";
                x.ExchangeType = "direct";
            });
        });

        cfg.ReceiveEndpoint("orders-email-queue", e =>
        {
            e.Bind("orders.broadcast", x => x.ExchangeType = "fanout");

            e.SetQueueArgument("x-message-ttl", 30000); // 30s
            e.SetQueueArgument("x-dead-letter-exchange", "dlx.exchange");
            e.SetQueueArgument("x-dead-letter-routing-key", "email.expired");

            e.UseInMemoryOutbox();
            e.ConfigureConsumer<EmailNotificationConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("orders-sms-queue", e =>
        {
            e.Bind("orders.broadcast", x => x.ExchangeType = "fanout");
            e.UseInMemoryOutbox();
            e.ConfigureConsumer<SmsNotificationConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("orders-email-queue.dlq", e =>
        {
            e.Bind("dlx.exchange", x =>
            {
                x.RoutingKey = "email.expired";
                x.ExchangeType = "direct";
            });
        });

        // ===== Topic: orders.topic -> orders-topic-audit (order.*) =====
        cfg.ReceiveEndpoint("orders-topic-audit", e =>
        {
            e.Bind("orders.topic", x =>
            {
                x.RoutingKey = "order.*";
                x.ExchangeType = "topic";
            });
        });

        cfg.ReceiveEndpoint("test-queue", e =>
        {
            e.Bind("test.exchange", x =>
            {
                x.RoutingKey = "test.key";
                x.ExchangeType = "direct";
            });
            e.SetQueueArgument("x-message-ttl", 30000); // 30s
            e.ConfigureConsumer<TestRabbitMqConsumer>(ctx);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

await builder.Build().RunAsync();