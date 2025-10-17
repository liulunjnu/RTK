using System;
using System.Collections.Generic;

namespace SemiE90
{
    /// <summary>
    /// 基板对象 - SEMI E90
    /// </summary>
    public class Substrate
    {
        public string SubstrateId { get; set; }
        public SubstrateType Type { get; set; }
        public SubstrateState State { get; set; }
        public SubstrateProcessResult ProcessResult { get; set; }
        
        public string CarrierId { get; set; }
        public int SlotNumber { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentStationId { get; set; }
        
        public DateTime ArrivalTime { get; set; }
        public DateTime StartProcessTime { get; set; }
        public DateTime EndProcessTime { get; set; }
        
        public string LotId { get; set; }
        public string RecipeId { get; set; }
        
        private List<SubstrateHistoryEntry> _history;

        public Substrate(string substrateId)
        {
            SubstrateId = substrateId;
            Type = SubstrateType.ProductWafer;
            State = SubstrateState.Unknown;
            ProcessResult = SubstrateProcessResult.NotProcessed;
            ArrivalTime = DateTime.Now;
            _history = new List<SubstrateHistoryEntry>();
        }

        public void UpdateState(SubstrateState newState, string location)
        {
            var oldState = State;
            State = newState;
            CurrentLocation = location;
            
            AddHistory(new SubstrateHistoryEntry
            {
                Timestamp = DateTime.Now,
                Event = $"State: {oldState} -> {newState}",
                Location = location
            });
        }

        public void AddHistory(SubstrateHistoryEntry entry)
        {
            _history.Add(entry);
        }

        public List<SubstrateHistoryEntry> GetHistory()
        {
            return new List<SubstrateHistoryEntry>(_history);
        }

        public override string ToString()
        {
            return $"Substrate[{SubstrateId}] State={State}, Location={CurrentLocation}";
        }
    }

    /// <summary>
    /// 基板历史记录
    /// </summary>
    public class SubstrateHistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
        public string Location { get; set; }
        public string StationId { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Event} @ {Location}";
        }
    }
}
