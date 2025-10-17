using System;

namespace SemiE84
{
    /// <summary>
    /// E84传输状态
    /// </summary>
    public enum E84State
    {
        /// <summary>0 - 传输中止</summary>
        TransferAborted = 0,
        /// <summary>1 - 未传输/就绪</summary>
        ReadyToTransfer = 1,
        /// <summary>2 - 传输请求</summary>
        TransferRequested = 2,
        /// <summary>3 - 传输就绪</summary>
        TransferReady = 3,
        /// <summary>4 - 传输中</summary>
        Transferring = 4,
        /// <summary>5 - 传输完成</summary>
        TransferComplete = 5
    }

    /// <summary>
    /// E84模式
    /// </summary>
    public enum E84Mode
    {
        /// <summary>主动模式 - 设备主动发起传输</summary>
        Active = 0,
        /// <summary>被动模式 - 设备等待传输</summary>
        Passive = 1
    }

    /// <summary>
    /// 载体交接方向
    /// </summary>
    public enum TransferDirection
    {
        /// <summary>载体装载到设备</summary>
        LoadToEquipment = 0,
        /// <summary>载体从设备卸载</summary>
        UnloadFromEquipment = 1
    }

    /// <summary>
    /// E84超时类型
    /// </summary>
    public enum E84Timeout
    {
        /// <summary>TP1 - VALID信号超时 (默认8秒)</summary>
        TP1_ValidSignal = 1,
        /// <summary>TP2 - CS_0信号超时 (默认8秒)</summary>
        TP2_CS0Signal = 2,
        /// <summary>TP3 - BUSY信号超时 (默认120秒)</summary>
        TP3_BusySignal = 3,
        /// <summary>TP4 - COMPT信号超时 (默认8秒)</summary>
        TP4_ComptSignal = 4,
        /// <summary>TP5 - CONT信号超时 (默认8秒)</summary>
        TP5_ContSignal = 5,
        /// <summary>TP6 - HO_AVBL信号超时 (默认无限)</summary>
        TP6_HoAvblSignal = 6
    }
}
