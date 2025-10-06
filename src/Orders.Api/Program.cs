using Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "dev", h =>
        {
            h.Username("admin");
            h.Password("admin_ChangeMe!");
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.MapGet("/", () => "Orders API is up (MassTransit configured).");

//Using RabbitMQ direct exchange
app.MapPost("/orders/submit", async (ISendEndpointProvider send, string customerId, decimal amount) =>
{
    var endpoint = await send.GetSendEndpoint(new Uri("exchange:orders.exchange?type=direct"));
    var msg = new SubmitOrder(Guid.NewGuid(), customerId, amount, DateTime.UtcNow);

    await endpoint.Send(msg, ctx =>
    {
        ctx.SetRoutingKey("order.submitted");
        ctx.Durable = true;
    });

    return Results.Accepted($"/orders/{msg.OrderId}", msg);
});

//Using RabbitMQ fanout exchange
app.MapPost("/order/brodcast", async (ISendEndpointProvider send, string message) =>
{
    var endpoint = await send.GetSendEndpoint(new Uri("exchange:orders.broadcast?type=fanout"));
    var orderId = Guid.NewGuid();
    var now = DateTime.UtcNow;

    await endpoint.Send(new OrderEmailNotification(orderId, "C1", message, now));
    return Results.Ok(new { Broadcast = "sent" });
});

//using RabbitMQ topic
app.MapPost("/topic/order/{verb}", async (ISendEndpointProvider send, string verb, string customerId, decimal amount) =>
{
    var endpoint = await send.GetSendEndpoint(new Uri("exchange:orders.topic?type=topic"));
    var rk = $"order.{verb}";
    var msg = new SubmitOrder(Guid.NewGuid(), customerId, amount, DateTime.UtcNow);

    await endpoint.Send(msg, ctx => ctx.SetRoutingKey(rk));
    return Results.Ok(new { Topic = rk, msg.OrderId });
});

app.MapPost("test/rabbitmq", async (ISendEndpointProvider send, string message) =>
{
    var endPoint = await send.GetSendEndpoint(new Uri("exchange:test.exchange?type=direct"));
    await endPoint.Send(new TestRabbitMq(message), ctx =>
    {
        ctx.SetRoutingKey("test.key");
        ctx.Durable = true;
    });
});

app.Run();