using System;

namespace GEM300.Events
{
    /// <summary>
    /// E84 - Material Received Event
    /// This event is triggered when material/substrate is received at the equipment
    /// </summary>
    public class E84MaterialReceived : Gem300EventBase
    {
        /// <summary>
        /// Material ID (Substrate/Lot ID)
        /// </summary>
        public string MaterialId { get; set; }
        
        /// <summary>
        /// Port ID where material was received
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
        /// Location code where material is placed
        /// </summary>
        public string LocationCode { get; set; }
        
        /// <summary>
        /// Material status
        /// </summary>
        public string MaterialStatus { get; set; }
        
        public E84MaterialReceived() : base("E84", "Material Received")
        {
        }
        
        public E84MaterialReceived(string equipmentId, string materialId, string portId, 
                                   string carrierId, int slotNumber, string locationCode, 
                                   string materialStatus = "RECEIVED") 
            : base("E84", "Material Received")
        {
            EquipmentId = equipmentId;
            MaterialId = materialId;
            PortId = portId;
            CarrierId = carrierId;
            SlotNumber = slotNumber;
            LocationCode = locationCode;
            MaterialStatus = materialStatus;
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
