using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gem300.Common;

public interface IEvent { DateTime Timestamp { get; } }

public interface ICommand { }

public sealed class EventEnvelope
{
    public required string Topic { get; init; }
    public required object Payload { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public sealed class EventBus
{
    private readonly ConcurrentDictionary<string, List<Channel<EventEnvelope>>> topicToSubscribers = new();

    public ChannelReader<EventEnvelope> Subscribe(string topic)
    {
        var channel = Channel.CreateUnbounded<EventEnvelope>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });
        var subscribers = topicToSubscribers.GetOrAdd(topic, _ => new List<Channel<EventEnvelope>>());
        lock (subscribers)
        {
            subscribers.Add(channel);
        }
        return channel.Reader;
    }

    public void Publish(string topic, object payload)
    {
        if (!topicToSubscribers.TryGetValue(topic, out var subscribers)) return;
        List<Channel<EventEnvelope>> snapshot;
        lock (subscribers)
        {
            snapshot = new List<Channel<EventEnvelope>>(subscribers);
        }
        var envelope = new EventEnvelope { Topic = topic, Payload = payload };
        foreach (var sub in snapshot)
        {
            sub.Writer.TryWrite(envelope);
        }
    }
}
