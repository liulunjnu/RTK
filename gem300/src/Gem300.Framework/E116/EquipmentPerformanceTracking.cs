using System;

namespace Gem300.Framework.E116
{
    public sealed class EquipmentMetrics
    {
        public TimeSpan ProductiveTime { get; private set; }
        public TimeSpan EngineeringTime { get; private set; }
        public TimeSpan StandbyTime { get; private set; }
        public TimeSpan ScheduledDownTime { get; private set; }
        public TimeSpan UnscheduledDownTime { get; private set; }

        public void AddProductive(TimeSpan t) { ProductiveTime += t; }
        public void AddEngineering(TimeSpan t) { EngineeringTime += t; }
        public void AddStandby(TimeSpan t) { StandbyTime += t; }
        public void AddScheduledDown(TimeSpan t) { ScheduledDownTime += t; }
        public void AddUnscheduledDown(TimeSpan t) { UnscheduledDownTime += t; }

        public double OEE { get { return TotalTime.TotalSeconds == 0 ? 0 : ProductiveTime.TotalSeconds / TotalTime.TotalSeconds; } }
        public TimeSpan TotalTime { get { return ProductiveTime + EngineeringTime + StandbyTime + ScheduledDownTime + UnscheduledDownTime; } }

        public EquipmentMetrics Clone()
        {
            var m = new EquipmentMetrics();
            m.ProductiveTime = this.ProductiveTime;
            m.EngineeringTime = this.EngineeringTime;
            m.StandbyTime = this.StandbyTime;
            m.ScheduledDownTime = this.ScheduledDownTime;
            m.UnscheduledDownTime = this.UnscheduledDownTime;
            return m;
        }
    }

    public sealed class EquipmentPerformanceTracker
    {
        private readonly EquipmentMetrics _metrics = new EquipmentMetrics();
        private DateTime _lastTick = DateTime.UtcNow;
        private string _currentCategory = "Standby";

        public EquipmentMetrics Snapshot() { return _metrics.Clone(); }

        public void SetCategory(string category)
        {
            Tick();
            _currentCategory = category;
        }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            var delta = now - _lastTick;
            _lastTick = now;
            switch (_currentCategory)
            {
                case "Productive": _metrics.AddProductive(delta); break;
                case "Engineering": _metrics.AddEngineering(delta); break;
                case "Standby": _metrics.AddStandby(delta); break;
                case "SDT": _metrics.AddScheduledDown(delta); break;
                case "UDT": _metrics.AddUnscheduledDown(delta); break;
                default: _metrics.AddStandby(delta); break;
            }
        }
    }
}
