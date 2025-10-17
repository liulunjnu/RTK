using System;
using System.Collections.Generic;

namespace GEM300.Events
{
    /// <summary>
    /// E94 - Control Job Status Event
    /// This event is triggered when a control job status changes
    /// Control Job manages the processing of materials through the equipment
    /// </summary>
    public class E94ControlJobStatus : Gem300EventBase
    {
        /// <summary>
        /// Control Job Status enumeration
        /// </summary>
        public enum ControlJobStatusType
        {
            QUEUED = 1,         // Job queued/waiting
            SELECTED = 2,       // Job selected
            WAITING = 3,        // Waiting for resources
            EXECUTING = 4,      // Job executing
            PAUSED = 5,         // Job paused
            COMPLETED = 6,      // Job completed successfully
            ABORTED = 7,        // Job aborted
            STOPPED = 8,        // Job stopped
            REJECTED = 9        // Job rejected
        }
        
        /// <summary>
        /// Equipment ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// Control Job ID
        /// </summary>
        public string ControlJobId { get; set; }
        
        /// <summary>
        /// Previous control job status
        /// </summary>
        public ControlJobStatusType PreviousStatus { get; set; }
        
        /// <summary>
        /// Current control job status
        /// </summary>
        public ControlJobStatusType CurrentStatus { get; set; }
        
        /// <summary>
        /// Process program ID/Recipe ID
        /// </summary>
        public string ProcessProgramId { get; set; }
        
        /// <summary>
        /// List of material IDs associated with this job
        /// </summary>
        public List<string> MaterialIds { get; set; }
        
        /// <summary>
        /// Start time of the control job
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// End time of the control job
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// Status change reason or comment
        /// </summary>
        public string StatusReason { get; set; }
        
        public E94ControlJobStatus() : base("E94", "Control Job Status")
        {
            MaterialIds = new List<string>();
        }
        
        public E94ControlJobStatus(string equipmentId, string controlJobId, 
                                   ControlJobStatusType previousStatus, 
                                   ControlJobStatusType currentStatus,
                                   string processProgramId, List<string> materialIds = null,
                                   string statusReason = "") 
            : base("E94", "Control Job Status")
        {
            EquipmentId = equipmentId;
            ControlJobId = controlJobId;
            PreviousStatus = previousStatus;
            CurrentStatus = currentStatus;
            ProcessProgramId = processProgramId;
            MaterialIds = materialIds ?? new List<string>();
            StatusReason = statusReason;
            
            if (currentStatus == ControlJobStatusType.EXECUTING && !StartTime.HasValue)
            {
                StartTime = DateTime.UtcNow;
            }
            
            if (currentStatus == ControlJobStatusType.COMPLETED || 
                currentStatus == ControlJobStatusType.ABORTED || 
                currentStatus == ControlJobStatusType.STOPPED)
            {
                EndTime = DateTime.UtcNow;
            }
        }
        
        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EquipmentId))
                return false;
                
            if (string.IsNullOrWhiteSpace(ControlJobId))
                return false;
                
            if (string.IsNullOrWhiteSpace(ProcessProgramId))
                return false;
                
            if (PreviousStatus == CurrentStatus)
                return false;
                
            return true;
        }
    }
}
