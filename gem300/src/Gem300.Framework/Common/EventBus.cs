using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Gem300.Framework.Common
{
    public interface IEvent { DateTime Timestamp { get; } }
    public interface ICommand { }

    public sealed class EventEnvelope
    {
        public string Topic { get; set; }
        public object Payload { get; set; }
        public DateTime Timestamp { get; set; }

        public EventEnvelope()
        {
            Timestamp = DateTime.UtcNow;
        }
    }

    public sealed class EventBus
    {
        private readonly ConcurrentDictionary<string, List<Action<EventEnvelope>>> _subscribers = new ConcurrentDictionary<string, List<Action<EventEnvelope>>>();

        public Action Subscribe(string topic, Action<EventEnvelope> handler)
        {
            var list = _subscribers.GetOrAdd(topic, _ => new List<Action<EventEnvelope>>());
            lock (list)
            {
                list.Add(handler);
            }
            return () =>
            {
                lock (list)
                {
                    list.Remove(handler);
                }
            };
        }

        public void Publish(string topic, object payload)
        {
            List<Action<EventEnvelope>> snapshot = null;
            if (_subscribers.TryGetValue(topic, out var list))
            {
                lock (list)
                {
                    snapshot = new List<Action<EventEnvelope>>(list);
                }
            }
            if (snapshot == null || snapshot.Count == 0) return;
            var envelope = new EventEnvelope { Topic = topic, Payload = payload };
            foreach (var h in snapshot) h(envelope);
        }
    }
}
