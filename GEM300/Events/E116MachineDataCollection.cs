using System;
using System.Collections.Generic;

namespace GEM300.Events
{
    /// <summary>
    /// E116 - Machine Data Collection Event
    /// This event is triggered to report collected equipment/process data
    /// </summary>
    public class E116MachineDataCollection : Gem300EventBase
    {
        /// <summary>
        /// Represents a single data item
        /// </summary>
        public class DataItem
        {
            public string DataId { get; set; }
            public string DataName { get; set; }
            public object Value { get; set; }
            public string Unit { get; set; }
            public DateTime CollectionTime { get; set; }
            
            public DataItem()
            {
                CollectionTime = DateTime.UtcNow;
            }
            
            public DataItem(string dataId, string dataName, object value, string unit = "")
            {
                DataId = dataId;
                DataName = dataName;
                Value = value;
                Unit = unit;
                CollectionTime = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// Equipment ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// Data collection report ID
        /// </summary>
        public string ReportId { get; set; }
        
        /// <summary>
        /// Collection event ID that triggered this report
        /// </summary>
        public string CollectionEventId { get; set; }
        
        /// <summary>
        /// Material ID (if data is associated with specific material)
        /// </summary>
        public string MaterialId { get; set; }
        
        /// <summary>
        /// Process program ID/Recipe ID
        /// </summary>
        public string ProcessProgramId { get; set; }
        
        /// <summary>
        /// List of collected data items
        /// </summary>
        public List<DataItem> DataItems { get; set; }
        
        /// <summary>
        /// Data collection category (e.g., "ProcessData", "EquipmentData", "AlarmData")
        /// </summary>
        public string DataCategory { get; set; }
        
        public E116MachineDataCollection() : base("E116", "Machine Data Collection")
        {
            DataItems = new List<DataItem>();
        }
        
        public E116MachineDataCollection(string equipmentId, string reportId, 
                                        string collectionEventId, string dataCategory,
                                        string materialId = "", string processProgramId = "") 
            : base("E116", "Machine Data Collection")
        {
            EquipmentId = equipmentId;
            ReportId = reportId;
            CollectionEventId = collectionEventId;
            DataCategory = dataCategory;
            MaterialId = materialId;
            ProcessProgramId = processProgramId;
            DataItems = new List<DataItem>();
        }
        
        /// <summary>
        /// Add a data item to the collection
        /// </summary>
        public void AddDataItem(string dataId, string dataName, object value, string unit = "")
        {
            DataItems.Add(new DataItem(dataId, dataName, value, unit));
        }
        
        /// <summary>
        /// Add a data item to the collection
        /// </summary>
        public void AddDataItem(DataItem item)
        {
            if (item != null)
            {
                DataItems.Add(item);
            }
        }
        
        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EquipmentId))
                return false;
                
            if (string.IsNullOrWhiteSpace(ReportId))
                return false;
                
            if (string.IsNullOrWhiteSpace(CollectionEventId))
                return false;
                
            if (string.IsNullOrWhiteSpace(DataCategory))
                return false;
                
            if (DataItems == null || DataItems.Count == 0)
                return false;
                
            return true;
        }
    }
}
