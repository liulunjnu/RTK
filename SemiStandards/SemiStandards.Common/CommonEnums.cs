using System;

namespace SemiStandards.Common
{
    /// <summary>
    /// 设备在线状态
    /// </summary>
    public enum EquipmentOnlineState
    {
        /// <summary>离线</summary>
        Offline = 0,
        /// <summary>在线本地</summary>
        OnlineLocal = 1,
        /// <summary>在线远程</summary>
        OnlineRemote = 2
    }

    /// <summary>
    /// 通信状态
    /// </summary>
    public enum CommunicationState
    {
        /// <summary>未连接</summary>
        NotConnected = 0,
        /// <summary>已连接</summary>
        Connected = 1,
        /// <summary>已选择</summary>
        Selected = 2,
        /// <summary>错误</summary>
        Error = 3
    }

    /// <summary>
    /// 处理状态
    /// </summary>
    public enum ProcessState
    {
        /// <summary>初始化中</summary>
        Initializing = 0,
        /// <summary>空闲</summary>
        Idle = 1,
        /// <summary>设置中</summary>
        Setup = 2,
        /// <summary>就绪</summary>
        Ready = 3,
        /// <summary>执行中</summary>
        Executing = 4,
        /// <summary>暂停</summary>
        Paused = 5,
        /// <summary>停止</summary>
        Stopped = 6
    }

    /// <summary>
    /// 对象服务事件类型 (E39)
    /// </summary>
    public enum ObjectEventType
    {
        /// <summary>对象创建</summary>
        Created = 1,
        /// <summary>对象修改</summary>
        Modified = 2,
        /// <summary>对象删除</summary>
        Deleted = 3,
        /// <summary>状态变更</summary>
        StateChanged = 4
    }

    /// <summary>
    /// 属性访问权限
    /// </summary>
    public enum AttributeAccess
    {
        /// <summary>只读</summary>
        ReadOnly = 1,
        /// <summary>读写</summary>
        ReadWrite = 2,
        /// <summary>只写</summary>
        WriteOnly = 3
    }
}
