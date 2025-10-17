using System;

namespace GEM300.Events
{
    /// <summary>
    /// E87 - Material Removed Event
    /// This event is triggered when material/substrate is removed from the equipment
    /// </summary>
    public class E87MaterialRemoved : Gem300EventBase
    {
        /// <summary>
        /// Material ID (Substrate/Lot ID)
        /// </summary>
        public string MaterialId { get; set; }
        
        /// <summary>
        /// Port ID where material was removed
        /// </summary>
        public string PortId { get; set; }
        
        /// <summary>
        /// Carrier ID
        /// </summary>
        public string CarrierId { get; set; }
        
        /// <summary>
        /// Slot number in the carrier
        /// </summary>
        public int SlotNumber { get; set; }
        
        /// <summary>
        /// Equipment ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// Location code where material was removed from
        /// </summary>
        public string LocationCode { get; set; }
        
        /// <summary>
        /// Material status after removal
        /// </summary>
        public string MaterialStatus { get; set; }
        
        /// <summary>
        /// Processing result (PASS, FAIL, etc.)
        /// </summary>
        public string ProcessingResult { get; set; }
        
        public E87MaterialRemoved() : base("E87", "Material Removed")
        {
        }
        
        public E87MaterialRemoved(string equipmentId, string materialId, string portId, 
                                  string carrierId, int slotNumber, string locationCode, 
                                  string materialStatus = "REMOVED", string processingResult = "PASS") 
            : base("E87", "Material Removed")
        {
            EquipmentId = equipmentId;
            MaterialId = materialId;
            PortId = portId;
            CarrierId = carrierId;
            SlotNumber = slotNumber;
            LocationCode = locationCode;
            MaterialStatus = materialStatus;
            ProcessingResult = processingResult;
        }
        
        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EquipmentId))
                return false;
                
            if (string.IsNullOrWhiteSpace(MaterialId))
                return false;
                
            if (string.IsNullOrWhiteSpace(PortId))
                return false;
                
            if (SlotNumber < 0)
                return false;
                
            return true;
        }
    }
}
