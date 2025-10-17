using System;
using System.Collections.Generic;
using System.Linq;

namespace GEM300
{
    /// <summary>
    /// Event handler delegate for GEM300 events
    /// </summary>
    public delegate void Gem300EventHandler(IGem300Event gemEvent);
    
    /// <summary>
    /// Manager class for handling GEM300 events
    /// Provides event registration, publication, and subscription functionality
    /// </summary>
    public class Gem300EventManager
    {
        private static readonly Lazy<Gem300EventManager> _instance = 
            new Lazy<Gem300EventManager>(() => new Gem300EventManager());
        
        private readonly Dictionary<string, List<Gem300EventHandler>> _eventHandlers;
        private readonly List<IGem300Event> _eventHistory;
        private readonly object _lockObject = new object();
        private readonly int _maxHistorySize;
        
        /// <summary>
        /// Singleton instance of the event manager
        /// </summary>
        public static Gem300EventManager Instance => _instance.Value;
        
        /// <summary>
        /// Event fired when any GEM300 event is published
        /// </summary>
        public event Gem300EventHandler OnEventPublished;
        
        private Gem300EventManager(int maxHistorySize = 1000)
        {
            _eventHandlers = new Dictionary<string, List<Gem300EventHandler>>();
            _eventHistory = new List<IGem300Event>();
            _maxHistorySize = maxHistorySize;
        }
        
        /// <summary>
        /// Subscribe to a specific event type
        /// </summary>
        /// <param name="eventId">Event ID (e.g., "E39", "E84")</param>
        /// <param name="handler">Event handler callback</param>
        public void Subscribe(string eventId, Gem300EventHandler handler)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException("Event ID cannot be null or empty", nameof(eventId));
            
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            
            lock (_lockObject)
            {
                if (!_eventHandlers.ContainsKey(eventId))
                {
                    _eventHandlers[eventId] = new List<Gem300EventHandler>();
                }
                
                _eventHandlers[eventId].Add(handler);
            }
        }
        
        /// <summary>
        /// Unsubscribe from a specific event type
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="handler">Event handler to remove</param>
        public void Unsubscribe(string eventId, Gem300EventHandler handler)
        {
            if (string.IsNullOrWhiteSpace(eventId) || handler == null)
                return;
            
            lock (_lockObject)
            {
                if (_eventHandlers.ContainsKey(eventId))
                {
                    _eventHandlers[eventId].Remove(handler);
                    
                    if (_eventHandlers[eventId].Count == 0)
                    {
                        _eventHandlers.Remove(eventId);
                    }
                }
            }
        }
        
        /// <summary>
        /// Publish a GEM300 event
        /// </summary>
        /// <param name="gemEvent">The event to publish</param>
        /// <returns>True if event was published successfully</returns>
        public bool PublishEvent(IGem300Event gemEvent)
        {
            if (gemEvent == null)
                throw new ArgumentNullException(nameof(gemEvent));
            
            if (!gemEvent.Validate())
            {
                throw new InvalidOperationException($"Event {gemEvent.EventId} validation failed");
            }
            
            lock (_lockObject)
            {
                // Add to history
                _eventHistory.Add(gemEvent);
                
                // Maintain history size limit
                if (_eventHistory.Count > _maxHistorySize)
                {
                    _eventHistory.RemoveAt(0);
                }
                
                // Notify general subscribers
                OnEventPublished?.Invoke(gemEvent);
                
                // Notify specific event type subscribers
                if (_eventHandlers.ContainsKey(gemEvent.EventId))
                {
                    foreach (var handler in _eventHandlers[gemEvent.EventId].ToList())
                    {
                        try
                        {
                            handler.Invoke(gemEvent);
                        }
                        catch (Exception ex)
                        {
                            // Log error but continue processing other handlers
                            Console.WriteLine($"Error in event handler for {gemEvent.EventId}: {ex.Message}");
                        }
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Get event history
        /// </summary>
        /// <param name="eventId">Optional: filter by event ID</param>
        /// <param name="count">Optional: limit number of events returned</param>
        /// <returns>List of events</returns>
        public List<IGem300Event> GetEventHistory(string eventId = null, int? count = null)
        {
            lock (_lockObject)
            {
                var query = _eventHistory.AsEnumerable();
                
                if (!string.IsNullOrWhiteSpace(eventId))
                {
                    query = query.Where(e => e.EventId == eventId);
                }
                
                query = query.OrderByDescending(e => e.Timestamp);
                
                if (count.HasValue)
                {
                    query = query.Take(count.Value);
                }
                
                return query.ToList();
            }
        }
        
        /// <summary>
        /// Get events within a time range
        /// </summary>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="eventId">Optional: filter by event ID</param>
        /// <returns>List of events</returns>
        public List<IGem300Event> GetEventsByTimeRange(DateTime startTime, DateTime endTime, string eventId = null)
        {
            lock (_lockObject)
            {
                var query = _eventHistory
                    .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime);
                
                if (!string.IsNullOrWhiteSpace(eventId))
                {
                    query = query.Where(e => e.EventId == eventId);
                }
                
                return query.OrderBy(e => e.Timestamp).ToList();
            }
        }
        
        /// <summary>
        /// Clear event history
        /// </summary>
        public void ClearHistory()
        {
            lock (_lockObject)
            {
                _eventHistory.Clear();
            }
        }
        
        /// <summary>
        /// Get count of subscribed handlers for an event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>Number of handlers</returns>
        public int GetSubscriberCount(string eventId)
        {
            lock (_lockObject)
            {
                return _eventHandlers.ContainsKey(eventId) ? _eventHandlers[eventId].Count : 0;
            }
        }
    }
}
