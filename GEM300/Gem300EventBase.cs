using System;
using System.Text.Json;

namespace GEM300
{
    /// <summary>
    /// Base class for all GEM300 events
    /// </summary>
    public abstract class Gem300EventBase : IGem300Event
    {
        public string EventId { get; protected set; }
        public string EventName { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        
        protected Gem300EventBase(string eventId, string eventName)
        {
            EventId = eventId;
            EventName = eventName;
            Timestamp = DateTime.UtcNow;
        }
        
        public virtual string Serialize()
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(this, this.GetType(), options);
        }
        
        public abstract bool Validate();
    }
}
