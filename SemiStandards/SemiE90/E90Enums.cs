using System;

namespace SemiE90
{
    /// <summary>
    /// 基板状态
    /// </summary>
    public enum SubstrateState
    {
        /// <summary>未知</summary>
        Unknown = 0,
        /// <summary>在载体中</summary>
        InCarrier = 1,
        /// <summary>在装载端口</summary>
        AtLoadPort = 2,
        /// <summary>在设备内</summary>
        InEquipment = 3,
        /// <summary>在工位</summary>
        AtStation = 4,
        /// <summary>处理中</summary>
        Processing = 5,
        /// <summary>已完成</summary>
        Completed = 6,
        /// <summary>异常</summary>
        Exception = 7
    }

    /// <summary>
    /// 基板类型
    /// </summary>
    public enum SubstrateType
    {
        /// <summary>产品晶圆</summary>
        ProductWafer = 0,
        /// <summary>测试晶圆</summary>
        TestWafer = 1,
        /// <summary>虚拟晶圆</summary>
        DummyWafer = 2,
        /// <summary>监控晶圆</summary>
        MonitorWafer = 3
    }

    /// <summary>
    /// 基板处理结果
    /// </summary>
    public enum SubstrateProcessResult
    {
        /// <summary>未处理</summary>
        NotProcessed = 0,
        /// <summary>成功</summary>
        Success = 1,
        /// <summary>失败</summary>
        Failed = 2,
        /// <summary>跳过</summary>
        Skipped = 3,
        /// <summary>中止</summary>
        Aborted = 4
    }
}
