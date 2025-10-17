using System;

namespace GEM300
{
    /// <summary>
    /// Base interface for all GEM300 events
    /// </summary>
    public interface IGem300Event
    {
        /// <summary>
        /// Event ID (e.g., E39, E84, etc.)
        /// </summary>
        string EventId { get; }
        
        /// <summary>
        /// Event name/description
        /// </summary>
        string EventName { get; }
        
        /// <summary>
        /// Timestamp when the event occurred
        /// </summary>
        DateTime Timestamp { get; }
        
        /// <summary>
        /// Serialize the event data
        /// </summary>
        /// <returns>Serialized event data</returns>
        string Serialize();
        
        /// <summary>
        /// Validate the event data
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        bool Validate();
    }
}
