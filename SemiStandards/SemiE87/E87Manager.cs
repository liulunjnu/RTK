using System;
using System.Collections.Generic;
using SemiStandards.Common;

namespace SemiE87
{
    public class E87Manager : IEventPublisher
    {
        private Dictionary<string, LoadPort> _loadPorts;
        private Dictionary<string, Carrier> _carriers;
        
        public string EquipmentId { get; private set; }
        public event EventHandler<SemiEventArgs> EventOccurred;

        public E87Manager(string equipmentId)
        {
            EquipmentId = equipmentId;
            _loadPorts = new Dictionary<string, LoadPort>();
            _carriers = new Dictionary<string, Carrier>();
        }

        public void AddLoadPort(string loadPortId)
        {
            _loadPorts[loadPortId] = new LoadPort(loadPortId);
        }

        public Carrier RegisterCarrier(string carrierId, int slotCount = 25)
        {
            if (!_carriers.ContainsKey(carrierId))
            {
                _carriers[carrierId] = new Carrier(carrierId, slotCount);
                PublishEvent(new SemiEventArgs("CarrierRegistered"));
            }
            return _carriers[carrierId];
        }

        public bool LoadCarrierToPort(string carrierId, string loadPortId)
        {
            if (!_carriers.ContainsKey(carrierId))
                throw new ArgumentException("Carrier not found");
            if (!_loadPorts.ContainsKey(loadPortId))
                throw new ArgumentException("LoadPort not found");

            var carrier = _carriers[carrierId];
            var loadPort = _loadPorts[loadPortId];
            
            bool result = loadPort.LoadCarrier(carrier);
            if (result)
            {
                PublishEvent(new SemiEventArgs("CarrierLoaded"));
            }
            return result;
        }

        public bool UnloadCarrierFromPort(string loadPortId)
        {
            if (!_loadPorts.ContainsKey(loadPortId))
                throw new ArgumentException("LoadPort not found");

            var loadPort = _loadPorts[loadPortId];
            bool result = loadPort.UnloadCarrier();
            
            if (result)
            {
                PublishEvent(new SemiEventArgs("CarrierUnloaded"));
            }
            return result;
        }

        public Carrier GetCarrier(string carrierId)
        {
            return _carriers.ContainsKey(carrierId) ? _carriers[carrierId] : null;
        }

        public LoadPort GetLoadPort(string loadPortId)
        {
            return _loadPorts.ContainsKey(loadPortId) ? _loadPorts[loadPortId] : null;
        }

        public void PublishEvent(SemiEventArgs eventArgs)
        {
            EventOccurred?.Invoke(this, eventArgs);
        }

        public string GetStatus()
        {
            return string.Format("E87 Manager [{0}]\nLoadPorts: {1}, Carriers: {2}", 
                EquipmentId, _loadPorts.Count, _carriers.Count);
        }
    }
}
