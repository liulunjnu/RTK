using System;
using System.Collections.Generic;

namespace SemiE87
{
    public class Carrier
    {
        public string CarrierId { get; set; }
        public CarrierIdStatus IdStatus { get; set; }
        public CarrierLocation Location { get; set; }
        public CarrierAccessMode AccessMode { get; set; }
        public string AssociatedLoadPortId { get; set; }
        public int SlotCount { get; private set; }
        public SlotMapStatus SlotMapStatus { get; set; }
        public string CarrierType { get; set; }
        
        private Dictionary<int, SlotOccupancyStatus> _slotMap;

        public Carrier(string carrierId, int slotCount = 25)
        {
            CarrierId = carrierId;
            SlotCount = slotCount;
            IdStatus = CarrierIdStatus.NotRead;
            Location = CarrierLocation.Unknown;
            AccessMode = CarrierAccessMode.Unknown;
            SlotMapStatus = SlotMapStatus.NotVerified;
            CarrierType = "FOUP";
            _slotMap = new Dictionary<int, SlotOccupancyStatus>();
            
            for (int i = 1; i <= slotCount; i++)
            {
                _slotMap[i] = SlotOccupancyStatus.Unknown;
            }
        }

        public void AssociateWithLoadPort(string loadPortId)
        {
            AssociatedLoadPortId = loadPortId;
            Location = CarrierLocation.AtLoadPort;
        }

        public void UpdateSlotMap(Dictionary<int, SlotOccupancyStatus> slotStatuses)
        {
            foreach (var kvp in slotStatuses)
            {
                if (_slotMap.ContainsKey(kvp.Key))
                {
                    _slotMap[kvp.Key] = kvp.Value;
                }
            }
            SlotMapStatus = SlotMapStatus.Verified;
        }

        public SlotOccupancyStatus GetSlotStatus(int slotNumber)
        {
            return _slotMap.ContainsKey(slotNumber) ? _slotMap[slotNumber] : SlotOccupancyStatus.Unknown;
        }

        public int GetOccupiedSlotCount()
        {
            int count = 0;
            foreach (var status in _slotMap.Values)
            {
                if (status == SlotOccupancyStatus.Occupied)
                    count++;
            }
            return count;
        }

        public override string ToString()
        {
            return string.Format("Carrier[{0}] Location={1}, Slots={2}/{3}", 
                CarrierId, Location, GetOccupiedSlotCount(), SlotCount);
        }
    }
}
