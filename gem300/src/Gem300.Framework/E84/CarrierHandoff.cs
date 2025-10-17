using System;
using Gem300.Framework.Common;

namespace Gem300.Framework.E84
{
    public enum E84Signal
    {
        HostReadyToLoad,
        HostReadyToUnload,
        EquipmentReady,
        DockRequest,
        UndockRequest,
        Complete
    }

    public sealed class CarrierHandoffStateMachine
    {
        private readonly object _sync = new object();

        public TransferState State { get; private set; }

        public event Action<TransferState> OnStateChanged;

        public CarrierHandoffStateMachine()
        {
            State = TransferState.Unknown;
        }

        private void SetState(TransferState newState)
        {
            if (State == newState) return;
            State = newState;
            var h = OnStateChanged; if (h != null) h(State);
        }

        public Result Handle(E84Signal signal)
        {
            lock (_sync)
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
                return Result.Fail("Invalid signal in state: " + State);
            }
        }
    }
}
