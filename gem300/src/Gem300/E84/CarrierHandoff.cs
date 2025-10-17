using System;
using System.Threading;
using System.Threading.Tasks;
using Gem300.Common;

namespace Gem300.E84;

public enum E84Signal
{
    // Simplified key signals
    HostReadyToLoad,
    HostReadyToUnload,
    EquipmentReady,
    DockRequest,
    UndockRequest,
    Complete
}

public sealed class CarrierHandoffStateMachine
{
    private readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(5);
    private readonly object sync = new();

    public TransferState State { get; private set; } = TransferState.Unknown;

    public event Action<TransferState>? OnStateChanged;

    private void SetState(TransferState newState)
    {
        if (State == newState) return;
        State = newState;
        OnStateChanged?.Invoke(State);
    }

    public Result Handle(E84Signal signal)
    {
        lock (sync)
        {
            switch (State)
            {
                case TransferState.Unknown:
                    if (signal == E84Signal.EquipmentReady) { SetState(TransferState.ReadyToLoad); return Result.Ok(); }
                    break;
                case TransferState.ReadyToLoad:
                    if (signal == E84Signal.DockRequest) { SetState(TransferState.Loading); return Result.Ok(); }
                    if (signal == E84Signal.HostReadyToUnload) { SetState(TransferState.ReadyToUnload); return Result.Ok(); }
                    break;
                case TransferState.Loading:
                    if (signal == E84Signal.Complete) { SetState(TransferState.Loaded); return Result.Ok(); }
                    break;
                case TransferState.Loaded:
                    if (signal == E84Signal.HostReadyToUnload) { SetState(TransferState.ReadyToUnload); return Result.Ok(); }
                    break;
                case TransferState.ReadyToUnload:
                    if (signal == E84Signal.UndockRequest) { SetState(TransferState.Unloading); return Result.Ok(); }
                    break;
                case TransferState.Unloading:
                    if (signal == E84Signal.Complete) { SetState(TransferState.Unknown); return Result.Ok(); }
                    break;
            }
            return Result.Fail($"Invalid signal {signal} in state {State}");
        }
    }
}
