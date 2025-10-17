using System;
using System.Collections.Generic;
using Gem300.Framework.Common;

namespace Gem300.Framework.E87
{
    public sealed class CarrierInfo
    {
        public CarrierId CarrierId { get; set; }
        public PortId PortId { get; set; }
        public CarrierState State { get; set; }
        public DateTime RegisteredAtUtc { get; private set; }
        public Dictionary<int, SubstrateId> SlotMap { get; private set; }

        public CarrierInfo()
        {
            State = CarrierState.Present;
            RegisteredAtUtc = DateTime.UtcNow;
            SlotMap = new Dictionary<int, SubstrateId>();
        }
    }

    public sealed class CarrierManager
    {
        private readonly Dictionary<string, CarrierInfo> _carriers = new Dictionary<string, CarrierInfo>();

        public event Action<CarrierInfo> OnCarrierRegistered;
        public event Action<CarrierInfo> OnCarrierRemoved;

        public Result RegisterCarrier(CarrierId carrierId, PortId portId, int slots)
        {
            if (_carriers.ContainsKey(carrierId.Value)) return Result.Fail("Carrier already registered");
            var info = new CarrierInfo { CarrierId = carrierId, PortId = portId };
            for (int i = 1; i <= slots; i++) info.SlotMap[i] = null;
            _carriers.Add(carrierId.Value, info);
            var h = OnCarrierRegistered; if (h != null) h(info);
            return Result.Ok();
        }

        public Result RemoveCarrier(CarrierId carrierId)
        {
            CarrierInfo info;
            if (!_carriers.TryGetValue(carrierId.Value, out info)) return Result.Fail("Carrier not found");
            _carriers.Remove(carrierId.Value);
            var h = OnCarrierRemoved; if (h != null) h(info);
            return Result.Ok();
        }

        public ResultOf<CarrierInfo> GetCarrier(CarrierId id)
        {
            CarrierInfo info; return _carriers.TryGetValue(id.Value, out info) ? ResultOf<CarrierInfo>.Ok(info) : ResultOf<CarrierInfo>.Fail("Not found");
        }

        public IReadOnlyCollection<CarrierInfo> ListCarriers() { return _carriers.Values; }
    }
}
