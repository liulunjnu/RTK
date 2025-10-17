using System;

namespace SemiE87
{
    public enum CarrierAccessMode { Unknown = 0, Auto = 1, Manual = 2 }
    public enum CarrierIdStatus { NotRead = 0, WaitingForRead = 1, IdRead = 2, IdVerified = 3, IdVerificationFailed = 4 }
    public enum LoadPortState { Uninitialized = 0, Empty = 1, Loading = 2, Loaded = 3, Unloading = 4, Error = 5 }
    public enum LoadPortTransferState { None = 0, TransferPaused = 1, ReadyToLoad = 2, ReadyToUnload = 3, Transferring = 4, TransferComplete = 5, TransferBlocked = 6 }
    public enum CarrierLocation { Unknown = 0, AtLoadPort = 1, InEquipment = 2, InTransit = 3, NotAtEquipment = 4 }
    public enum SlotMapStatus { NotVerified = 0, WaitingForVerification = 1, Verified = 2, VerificationFailed = 3 }
    public enum SlotOccupancyStatus { Empty = 0, Occupied = 1, DoubleSlotted = 2, CrossSlotted = 3, Unknown = 4 }
}
