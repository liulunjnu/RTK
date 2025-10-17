using System;

namespace GEM300.Events
{
    /// <summary>
    /// E90 - Spooling Activity Event
    /// This event is triggered during data spooling activities (file transfer, data logging, etc.)
    /// </summary>
    public class E90SpoolingActivity : Gem300EventBase
    {
        /// <summary>
        /// Spooling activity type enumeration
        /// </summary>
        public enum SpoolingActivityType
        {
            START = 1,      // Spooling started
            ACTIVE = 2,     // Spooling active/in progress
            COMPLETE = 3,   // Spooling completed successfully
            FAILED = 4,     // Spooling failed
            ABORTED = 5,    // Spooling aborted
            PAUSED = 6      // Spooling paused
        }
        
        /// <summary>
        /// Equipment ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// Spooling stream ID
        /// </summary>
        public string SpoolStreamId { get; set; }
        
        /// <summary>
        /// Spooling activity type
        /// </summary>
        public SpoolingActivityType ActivityType { get; set; }
        
        /// <summary>
        /// Data type being spooled (e.g., "ProcessData", "AlarmLog", "EventLog")
        /// </summary>
        public string DataType { get; set; }
        
        /// <summary>
        /// Number of records spooled
        /// </summary>
        public long RecordCount { get; set; }
        
        /// <summary>
        /// File path or destination
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// Error message (if activity failed)
        /// </summary>
        public string ErrorMessage { get; set; }
        
        public E90SpoolingActivity() : base("E90", "Spooling Activity")
        {
        }
        
        public E90SpoolingActivity(string equipmentId, string spoolStreamId, 
                                   SpoolingActivityType activityType, string dataType, 
                                   long recordCount = 0, string destination = "", 
                                   string errorMessage = "") 
            : base("E90", "Spooling Activity")
        {
            EquipmentId = equipmentId;
            SpoolStreamId = spoolStreamId;
            ActivityType = activityType;
            DataType = dataType;
            RecordCount = recordCount;
            Destination = destination;
            ErrorMessage = errorMessage;
        }
        
        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EquipmentId))
                return false;
                
            if (string.IsNullOrWhiteSpace(SpoolStreamId))
                return false;
                
            if (string.IsNullOrWhiteSpace(DataType))
                return false;
                
            if (ActivityType == SpoolingActivityType.FAILED && string.IsNullOrWhiteSpace(ErrorMessage))
                return false;
                
            return true;
        }
    }
}
