namespace Gem300.Common;

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

public readonly record struct CarrierId(string Value)
{
    public override string ToString() => Value;
}

public readonly record struct SubstrateId(string Value)
{
    public override string ToString() => Value;
}

public readonly record struct PortId(string Value)
{
    public override string ToString() => Value;
}

public readonly record struct Slot(int Value)
{
    public override string ToString() => Value.ToString();
}
