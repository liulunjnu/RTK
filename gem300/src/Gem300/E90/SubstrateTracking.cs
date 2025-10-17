using System;
using System.Collections.Generic;
using Gem300.Common;

namespace Gem300.E90;

public enum SubstrateLocationType { CarrierSlot, ProcessModule }

public sealed class SubstrateLocation
{
    public SubstrateLocationType Type { get; init; }
    public CarrierId? CarrierId { get; init; }
    public Slot? Slot { get; init; }
    public string? ModuleId { get; init; }
}

public sealed class SubstrateRecord
{
    public required SubstrateId SubstrateId { get; init; }
    public required SubstrateLocation Location { get; set; }
    public List<(DateTime at, SubstrateLocation from, SubstrateLocation to)> History { get; } = new();
}

public sealed class SubstrateTracker
{
    private readonly Dictionary<SubstrateId, SubstrateRecord> records = new();

    public Result RegisterInCarrier(SubstrateId id, CarrierId carrierId, Slot slot)
    {
        if (records.ContainsKey(id)) return Result.Fail("Already registered");
        var loc = new SubstrateLocation { Type = SubstrateLocationType.CarrierSlot, CarrierId = carrierId, Slot = slot };
        records[id] = new SubstrateRecord { SubstrateId = id, Location = loc };
        return Result.Ok();
    }

    public Result MoveToModule(SubstrateId id, string moduleId)
    {
        if (!records.TryGetValue(id, out var rec)) return Result.Fail("Unknown substrate");
        var from = rec.Location;
        var to = new SubstrateLocation { Type = SubstrateLocationType.ProcessModule, ModuleId = moduleId };
        rec.Location = to;
        rec.History.Add((DateTime.UtcNow, from, to));
        return Result.Ok();
    }

    public Result ReturnToCarrier(SubstrateId id, CarrierId carrierId, Slot slot)
    {
        if (!records.TryGetValue(id, out var rec)) return Result.Fail("Unknown substrate");
        var from = rec.Location;
        var to = new SubstrateLocation { Type = SubstrateLocationType.CarrierSlot, CarrierId = carrierId, Slot = slot };
        rec.Location = to;
        rec.History.Add((DateTime.UtcNow, from, to));
        return Result.Ok();
    }

    public Result<SubstrateRecord> Get(SubstrateId id)
    {
        return records.TryGetValue(id, out var rec) ? Result<SubstrateRecord>.Ok(rec) : Result<SubstrateRecord>.Fail("Not found");
    }
}
