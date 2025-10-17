using System;
using System.Collections.Generic;
using Gem300.Common;

namespace Gem300.E87;

public sealed class CarrierInfo
{
    public required CarrierId CarrierId { get; init; }
    public required PortId PortId { get; set; }
    public CarrierState State { get; set; } = CarrierState.Present;
    public DateTime RegisteredAtUtc { get; } = DateTime.UtcNow;
    public Dictionary<Slot, SubstrateId?> SlotMap { get; } = new();
}

public sealed class CarrierManager
{
    private readonly Dictionary<CarrierId, CarrierInfo> carriers = new();

    public event Action<CarrierInfo>? OnCarrierRegistered;
    public event Action<CarrierInfo>? OnCarrierRemoved;

    public Result RegisterCarrier(CarrierId carrierId, PortId portId, int slots)
    {
        if (carriers.ContainsKey(carrierId)) return Result.Fail("Carrier already registered");
        var info = new CarrierInfo { CarrierId = carrierId, PortId = portId };
        for (int i = 1; i <= slots; i++) info.SlotMap[new Slot(i)] = null;
        carriers.Add(carrierId, info);
        OnCarrierRegistered?.Invoke(info);
        return Result.Ok();
    }

    public Result RemoveCarrier(CarrierId carrierId)
    {
        if (!carriers.Remove(carrierId, out var info)) return Result.Fail("Carrier not found");
        OnCarrierRemoved?.Invoke(info);
        return Result.Ok();
    }

    public Result<CarrierInfo> GetCarrier(CarrierId id)
    {
        return carriers.TryGetValue(id, out var info) ? Result<CarrierInfo>.Ok(info) : Result<CarrierInfo>.Fail("Not found");
    }

    public IReadOnlyCollection<CarrierInfo> ListCarriers() => carriers.Values;
}
