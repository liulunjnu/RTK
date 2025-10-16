using System;

namespace SemiE87
{
    /// <summary>
    /// 载体槽位信息 (Carrier Slot Information)
    /// </summary>
    public class CarrierSlot
    {
        /// <summary>槽位编号 (1-based)</summary>
        public int SlotNumber { get; set; }

        /// <summary>槽位占用状态</summary>
        public SlotOccupancyStatus OccupancyStatus { get; set; }

        /// <summary>基板ID (Substrate ID) - 如果有</summary>
        public string SubstrateId { get; set; }

        /// <summary>基板关联状态</summary>
        public SubstrateAssociationState SubstrateAssociation { get; set; }

        public CarrierSlot(int slotNumber)
        {
            SlotNumber = slotNumber;
            OccupancyStatus = SlotOccupancyStatus.Unknown;
            SubstrateId = string.Empty;
            SubstrateAssociation = SubstrateAssociationState.NotAssociated;
        }

        public override string ToString()
        {
            return $"Slot {SlotNumber}: {OccupancyStatus}" +
                   (string.IsNullOrEmpty(SubstrateId) ? "" : $" (SubstrateID: {SubstrateId})");
        }
    }
}
