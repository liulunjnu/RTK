using System;

namespace SemiE94
{
    /// <summary>
    /// 控制作业状态
    /// </summary>
    public enum ControlJobState
    {
        /// <summary>排队等待</summary>
        Queued = 1,
        /// <summary>已选中</summary>
        Selected = 2,
        /// <summary>等待开始</summary>
        WaitingForStart = 3,
        /// <summary>执行中</summary>
        Executing = 4,
        /// <summary>已暂停</summary>
        Paused = 5,
        /// <summary>已完成</summary>
        Completed = 6,
        /// <summary>已取消</summary>
        Canceled = 7
    }

    /// <summary>
    /// 处理作业状态
    /// </summary>
    public enum ProcessJobState
    {
        /// <summary>等待处理</summary>
        WaitingForProcessing = 1,
        /// <summary>处理中</summary>
        Processing = 2,
        /// <summary>处理完成</summary>
        ProcessingComplete = 3,
        /// <summary>已跳过</summary>
        Skipped = 4,
        /// <summary>已取消</summary>
        Canceled = 5,
        /// <summary>已停止</summary>
        Stopped = 6,
        /// <summary>已中止</summary>
        Aborted = 7,
        /// <summary>已拒绝</summary>
        Rejected = 8
    }

    /// <summary>
    /// 载体作业状态
    /// </summary>
    public enum CarrierJobState
    {
        /// <summary>等待开始</summary>
        WaitingForStart = 1,
        /// <summary>处理中</summary>
        Processing = 2,
        /// <summary>已完成</summary>
        Complete = 3,
        /// <summary>已暂停</summary>
        Paused = 4,
        /// <summary>已取消</summary>
        Canceled = 5
    }
}
