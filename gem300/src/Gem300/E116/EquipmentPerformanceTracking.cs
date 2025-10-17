using System;
using System.Collections.Generic;

namespace Gem300.E116;

public sealed class EquipmentMetrics
{
    public TimeSpan ProductiveTime { get; private set; }
    public TimeSpan EngineeringTime { get; private set; }
    public TimeSpan StandbyTime { get; private set; }
    public TimeSpan ScheduledDownTime { get; private set; }
    public TimeSpan UnscheduledDownTime { get; private set; }

    public void AddProductive(TimeSpan t) => ProductiveTime += t;
    public void AddEngineering(TimeSpan t) => EngineeringTime += t;
    public void AddStandby(TimeSpan t) => StandbyTime += t;
    public void AddScheduledDown(TimeSpan t) => ScheduledDownTime += t;
    public void AddUnscheduledDown(TimeSpan t) => UnscheduledDownTime += t;

    public double OEE => TotalTime.TotalSeconds == 0 ? 0 : ProductiveTime.TotalSeconds / TotalTime.TotalSeconds;
    public TimeSpan TotalTime => ProductiveTime + EngineeringTime + StandbyTime + ScheduledDownTime + UnscheduledDownTime;

    public EquipmentMetrics Clone()
    {
        return new EquipmentMetrics
        {
            ProductiveTime = this.ProductiveTime,
            EngineeringTime = this.EngineeringTime,
            StandbyTime = this.StandbyTime,
            ScheduledDownTime = this.ScheduledDownTime,
            UnscheduledDownTime = this.UnscheduledDownTime
        };
    }
}

public sealed class EquipmentPerformanceTracker
{
    private readonly EquipmentMetrics metrics = new();
    private DateTime lastTick = DateTime.UtcNow;
    private string currentCategory = "Standby"; // Simplified category label

    public EquipmentMetrics Snapshot() => metrics.Clone();

    public void SetCategory(string category)
    {
        Tick();
        currentCategory = category;
    }

    public void Tick()
    {
        var now = DateTime.UtcNow;
        var delta = now - lastTick;
        lastTick = now;
        switch (currentCategory)
        {
            case "Productive": metrics.AddProductive(delta); break;
            case "Engineering": metrics.AddEngineering(delta); break;
            case "Standby": metrics.AddStandby(delta); break;
            case "SDT": metrics.AddScheduledDown(delta); break;
            case "UDT": metrics.AddUnscheduledDown(delta); break;
            default: metrics.AddStandby(delta); break;
        }
    }
}
