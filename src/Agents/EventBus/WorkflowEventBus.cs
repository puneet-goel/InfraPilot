using Agents.Workflow;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Agents.EventBus;

public sealed class WorkflowEventBus
{
    private readonly ConcurrentDictionary<Guid,ConcurrentDictionary<Guid, Channel<WorkflowEvent>>> _connections = new();

    public (
        Guid SubscriptionId,
        ChannelReader<WorkflowEvent> Reader
    ) Subscribe(Guid executionId)
    {
        Guid subscriptionId = Guid.NewGuid();

        Channel<WorkflowEvent> channel = Channel.CreateUnbounded<WorkflowEvent>();

        ConcurrentDictionary<Guid, Channel<WorkflowEvent>> subscribers =
            _connections.GetOrAdd(executionId, _ => new());

        subscribers[subscriptionId] = channel;

        return (
            subscriptionId,
            channel.Reader
        );
    }

    public async Task PublishAsync(Guid executionId, WorkflowEvent evt)
    {
        if (!_connections.TryGetValue(executionId, out var subscribers))
        {
            return;
        }

        List<Guid> disconnected = [];

        foreach (var subscriber in subscribers)
        {
            try
            {
                await subscriber.Value
                    .Writer
                    .WriteAsync(evt);
            }
            catch
            {
                disconnected.Add(subscriber.Key);
            }
        }

        // cleanup dead subscribers
        foreach (Guid id in disconnected)
        {
            subscribers.TryRemove(id, out _);
        }
    }

    public void Unsubscribe(Guid executionId, Guid? subscriptionId)
    {
        if (!_connections.TryGetValue(executionId, out var subscribers))
        {
            return;
        }

        if (subscriptionId != null)
        {
            subscribers.TryRemove(subscriptionId ?? new Guid(), out _);
        } else
        {
            subscribers.Clear();
        }

        // remove workflow bucket
        // if no subscribers left
        if (subscribers.IsEmpty)
        {
            _connections.TryRemove(executionId, out _);
        }
    }
}