using System;

namespace GEM300.Events
{
    /// <summary>
    /// E39 - Processing State Change Event
    /// This event is triggered when the equipment's processing state changes
    /// </summary>
    public class E39ProcessingStateChange : Gem300EventBase
    {
        /// <summary>
        /// Processing state enumeration
        /// </summary>
        public enum ProcessingState
        {
            INIT = 1,           // Initialization
            IDLE = 2,           // Idle/Ready
            SETUP = 3,          // Setup
            READY = 4,          // Ready to process
            EXECUTING = 5,      // Executing/Processing
            PAUSE = 6,          // Paused
            COMPLETE = 7        // Complete
        }
        
        /// <summary>
        /// Previous processing state
        /// </summary>
        public ProcessingState PreviousState { get; set; }
        
        /// <summary>
        /// Current processing state
        /// </summary>
        public ProcessingState CurrentState { get; set; }
        
        /// <summary>
        /// Equipment ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// Optional state change reason
        /// </summary>
        public string Reason { get; set; }
        
        public E39ProcessingStateChange() : base("E39", "Processing State Change")
        {
        }
        
        public E39ProcessingStateChange(string equipmentId, ProcessingState previousState, 
                                        ProcessingState currentState, string reason = "") 
            : base("E39", "Processing State Change")
        {
            EquipmentId = equipmentId;
            PreviousState = previousState;
            CurrentState = currentState;
            Reason = reason;
        }
        
        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EquipmentId))
                return false;
                
            if (PreviousState == CurrentState)
                return false;
                
            return true;
        }
    }
}
