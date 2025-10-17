using System;

namespace SemiStandards.Common
{
    /// <summary>
    /// SEMI标准通用事件参数基类
    /// </summary>
    public class SemiEventArgs : EventArgs
    {
        public DateTime EventTime { get; set; }
        public string EventName { get; set; }

        public SemiEventArgs()
        {
            EventTime = DateTime.Now;
        }

        public SemiEventArgs(string eventName) : this()
        {
            EventName = eventName;
        }
    }

    /// <summary>
    /// 对象事件参数
    /// </summary>
    public class ObjectEventArgs : SemiEventArgs
    {
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public ObjectEventType EventType { get; set; }

        public ObjectEventArgs(string objectId, string objectType, ObjectEventType eventType)
            : base($"Object{eventType}")
        {
            ObjectId = objectId;
            ObjectType = objectType;
            EventType = eventType;
        }
    }

    /// <summary>
    /// 状态变更事件参数
    /// </summary>
    public class StateChangedEventArgs : SemiEventArgs
    {
        public string PreviousState { get; set; }
        public string CurrentState { get; set; }

        public StateChangedEventArgs(string previousState, string currentState)
            : base("StateChanged")
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
