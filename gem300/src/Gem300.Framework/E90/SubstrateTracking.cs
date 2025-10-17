using System;
using System.Collections.Generic;
using Gem300.Framework.Common;

namespace Gem300.Framework.E90
{
    public enum SubstrateLocationType { CarrierSlot, ProcessModule }

    public sealed class SubstrateLocation
    {
        public SubstrateLocationType Type { get; set; }
        public CarrierId CarrierId { get; set; }
        public Slot Slot { get; set; }
        public string ModuleId { get; set; }
    }

    public sealed class MovementEntry
    {
        public DateTime At { get; set; }
        public SubstrateLocation From { get; set; }
        public SubstrateLocation To { get; set; }
    }

    public sealed class SubstrateRecord
    {
        public SubstrateId SubstrateId { get; set; }
        public SubstrateLocation Location { get; set; }
        public List<MovementEntry> History { get; private set; }
        public SubstrateRecord() { History = new List<MovementEntry>(); }
    }

    public sealed class SubstrateTracker
    {
        private readonly Dictionary<string, SubstrateRecord> _records = new Dictionary<string, SubstrateRecord>();

        public Result RegisterInCarrier(SubstrateId id, CarrierId carrierId, Slot slot)
        {
            if (_records.ContainsKey(id.Value)) return Result.Fail("Already registered");
            var loc = new SubstrateLocation { Type = SubstrateLocationType.CarrierSlot, CarrierId = carrierId, Slot = slot };
            _records[id.Value] = new SubstrateRecord { SubstrateId = id, Location = loc };
            return Result.Ok();
        }

        public Result MoveToModule(SubstrateId id, string moduleId)
        {
            SubstrateRecord rec; if (!_records.TryGetValue(id.Value, out rec)) return Result.Fail("Unknown substrate");
            var from = rec.Location;
            var to = new SubstrateLocation { Type = SubstrateLocationType.ProcessModule, ModuleId = moduleId };
            rec.Location = to;
            rec.History.Add(new MovementEntry { At = DateTime.UtcNow, From = from, To = to });
            return Result.Ok();
        }

        public Result ReturnToCarrier(SubstrateId id, CarrierId carrierId, Slot slot)
        {
            SubstrateRecord rec; if (!_records.TryGetValue(id.Value, out rec)) return Result.Fail("Unknown substrate");
            var from = rec.Location;
            var to = new SubstrateLocation { Type = SubstrateLocationType.CarrierSlot, CarrierId = carrierId, Slot = slot };
            rec.Location = to;
            rec.History.Add(new MovementEntry { At = DateTime.UtcNow, From = from, To = to });
            return Result.Ok();
        }

        public ResultOf<SubstrateRecord> Get(SubstrateId id)
        {
            SubstrateRecord rec; return _records.TryGetValue(id.Value, out rec) ? ResultOf<SubstrateRecord>.Ok(rec) : ResultOf<SubstrateRecord>.Fail("Not found");
        }
    }
}
