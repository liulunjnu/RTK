using System;
using System.Collections.Generic;

namespace SemiStandards.Common
{
    /// <summary>
    /// SEMI对象接口 (E39)
    /// </summary>
    public interface ISemiObject
    {
        /// <summary>对象标识符</summary>
        string ObjectId { get; }

        /// <summary>对象类型</summary>
        string ObjectType { get; }

        /// <summary>获取属性值</summary>
        object GetAttribute(string attributeName);

        /// <summary>设置属性值</summary>
        void SetAttribute(string attributeName, object value);

        /// <summary>获取所有属性</summary>
        IDictionary<string, object> GetAllAttributes();
    }

    /// <summary>
    /// 事件发布接口
    /// </summary>
    public interface IEventPublisher
    {
        event EventHandler<SemiEventArgs> EventOccurred;
        void PublishEvent(SemiEventArgs eventArgs);
    }

    /// <summary>
    /// 状态机接口
    /// </summary>
    public interface IStateMachine
    {
        string CurrentState { get; }
        bool TransitionTo(string newState);
        IEnumerable<string> GetValidTransitions();
    }
}
