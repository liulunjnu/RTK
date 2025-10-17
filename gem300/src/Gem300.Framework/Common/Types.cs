namespace Gem300.Framework.Common
{
    public enum TransferState
    {
        Unknown = 0,
        ReadyToLoad,
        Loading,
        Loaded,
        ReadyToUnload,
        Unloading
    }

    public enum CarrierState
    {
        Unknown = 0,
        Present,
        Absent,
        Busy,
        Error
    }

    public sealed class CarrierId
    {
        public CarrierId(string value) { Value = value; }
        public string Value { get; }
        public override string ToString() => Value;
    }

    public sealed class SubstrateId
    {
        public SubstrateId(string value) { Value = value; }
        public string Value { get; }
        public override string ToString() => Value;
    }

    public sealed class PortId
    {
        public PortId(string value) { Value = value; }
        public string Value { get; }
        public override string ToString() => Value;
    }

    public sealed class Slot
    {
        public Slot(int value) { Value = value; }
        public int Value { get; }
        public override string ToString() => Value.ToString();
    }
}
