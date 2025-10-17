using System;

namespace SemiE87
{
    /// <summary>
    /// 载体访问模式 (Carrier Access Mode)
    /// </summary>
    public enum CarrierAccessMode
    {
        /// <summary>未知</summary>
        Unknown = 0,
        /// <summary>自动模式 - 设备可以自动访问载体</summary>
        Auto = 1,
        /// <summary>手动模式 - 需要操作员确认才能访问</summary>
        Manual = 2
    }

    /// <summary>
    /// 载体ID状态 (Carrier ID Status)
    /// </summary>
    public enum CarrierIdStatus
    {
        /// <summary>未读取</summary>
        NotRead = 0,
        /// <summary>等待读取</summary>
        WaitingForRead = 1,
        /// <summary>已读取</summary>
        IdRead = 2,
        /// <summary>ID验证通过</summary>
        IdVerified = 3,
        /// <summary>ID验证失败</summary>
        IdVerificationFailed = 4
    }

    /// <summary>
    /// 装载端口状态 (Load Port State)
    /// </summary>
    public enum LoadPortState
    {
        /// <summary>未初始化</summary>
        Uninitialized = 0,
        /// <summary>空闲，无载体</summary>
        Empty = 1,
        /// <summary>正在装载</summary>
        Loading = 2,
        /// <summary>已装载，载体在位</summary>
        Loaded = 3,
        /// <summary>正在卸载</summary>
        Unloading = 4,
        /// <summary>错误状态</summary>
        Error = 5
    }

    /// <summary>
    /// 装载端口传输状态 (Load Port Transfer State)
    /// </summary>
    public enum LoadPortTransferState
    {
        /// <summary>未传输</summary>
        None = 0,
        /// <summary>传输已暂停</summary>
        TransferPaused = 1,
        /// <summary>就绪传输</summary>
        ReadyToLoad = 2,
        /// <summary>就绪卸载</summary>
        ReadyToUnload = 3,
        /// <summary>传输中</summary>
        Transferring = 4,
        /// <summary>传输完成</summary>
        TransferComplete = 5,
        /// <summary>传输被阻止</summary>
        TransferBlocked = 6
    }

    /// <summary>
    /// 载体位置 (Carrier Location)
    /// </summary>
    public enum CarrierLocation
    {
        /// <summary>未知位置</summary>
        Unknown = 0,
        /// <summary>在装载端口</summary>
        AtLoadPort = 1,
        /// <summary>在设备内部</summary>
        InEquipment = 2,
        /// <summary>在传输中</summary>
        InTransit = 3,
        /// <summary>不在设备</summary>
        NotAtEquipment = 4
    }

    /// <summary>
    /// 载体关联状态 (Carrier Association State)
    /// </summary>
    public enum CarrierAssociationState
    {
        /// <summary>未关联</summary>
        NotAssociated = 0,
        /// <summary>已关联到装载端口</summary>
        AssociatedWithLoadPort = 1,
        /// <summary>已关联到设备</summary>
        AssociatedWithEquipment = 2
    }

    /// <summary>
    /// 基板关联状态 (Substrate Association State)
    /// SEMI E90 集成
    /// </summary>
    public enum SubstrateAssociationState
    {
        /// <summary>未关联</summary>
        NotAssociated = 0,
        /// <summary>已关联</summary>
        Associated = 1,
        /// <summary>部分关联</summary>
        PartiallyAssociated = 2
    }

    /// <summary>
    /// Slot 映射状态 (Slot Map Status)
    /// </summary>
    public enum SlotMapStatus
    {
        /// <summary>未验证</summary>
        NotVerified = 0,
        /// <summary>等待验证</summary>
        WaitingForVerification = 1,
        /// <summary>已验证</summary>
        Verified = 2,
        /// <summary>验证失败</summary>
        VerificationFailed = 3
    }

    /// <summary>
    /// Slot 占用状态 (Slot Occupancy Status)
    /// </summary>
    public enum SlotOccupancyStatus
    {
        /// <summary>空</summary>
        Empty = 0,
        /// <summary>已占用</summary>
        Occupied = 1,
        /// <summary>双重占用</summary>
        DoubleSlotted = 2,
        /// <summary>交叉占用</summary>
        CrossSlotted = 3,
        /// <summary>未知</summary>
        Unknown = 4
    }

    /// <summary>
    /// 载体锁定状态 (Carrier Clamp State)
    /// </summary>
    public enum ClampState
    {
        /// <summary>未知</summary>
        Unknown = 0,
        /// <summary>未夹紧</summary>
        Unclamped = 1,
        /// <summary>已夹紧</summary>
        Clamped = 2,
        /// <summary>错误</summary>
        Error = 3
    }

    /// <summary>
    /// 门状态 (Door State)
    /// </summary>
    public enum DoorState
    {
        /// <summary>未知</summary>
        Unknown = 0,
        /// <summary>关闭</summary>
        Closed = 1,
        /// <summary>打开</summary>
        Open = 2,
        /// <summary>正在打开</summary>
        Opening = 3,
        /// <summary>正在关闭</summary>
        Closing = 4,
        /// <summary>错误</summary>
        Error = 5
    }
}
