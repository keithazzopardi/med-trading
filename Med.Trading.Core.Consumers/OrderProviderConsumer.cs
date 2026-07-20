using Med.Trading.Core.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Med.Trading.Core.Consumers;

public sealed class OrderProviderConsumer(
    IKafkaConsumer<OrderCreatedEvent> consumer,
    ILogger<OrderProviderConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var order = consumer.Consume(stoppingToken);
                if (order is null)
                    continue;

                await SendToProviderAsync(order, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
        finally
        {
            consumer.Dispose();
        }
    }

    private static Task SendToProviderAsync(
        OrderCreatedEvent order, CancellationToken cancellationToken)
    {
        // TODO: Replace this mock with the external trading-provider client call.
        return Task.CompletedTask;
    }
}
