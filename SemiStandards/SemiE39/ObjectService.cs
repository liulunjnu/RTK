using System;
using System.Collections.Generic;
using SemiStandards.Common;

namespace SemiE39
{
    /// <summary>
    /// E39对象服务管理器
    /// </summary>
    public class ObjectService : IEventPublisher
    {
        private ObjectRepository _repository;
        
        public string ServiceName { get; private set; }
        public event EventHandler<SemiEventArgs> EventOccurred;

        public ObjectService(string serviceName)
        {
            ServiceName = serviceName;
            _repository = new ObjectRepository();

            // 订阅仓库事件
            _repository.ObjectCreated += OnRepositoryEvent;
            _repository.ObjectDeleted += OnRepositoryEvent;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        public SemiObject CreateObject(string objectId, string objectType, Dictionary<string, object> attributes = null)
        {
            var obj = _repository.CreateObject(objectId, objectType);

            // 添加自定义属性
            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    obj.AddAttribute(attr.Key, attr.Value);
                }
            }

            return obj;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public SemiObject GetObject(string objectId)
        {
            return _repository.GetObject(objectId);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public bool DeleteObject(string objectId)
        {
            return _repository.DeleteObject(objectId);
        }

        /// <summary>
        /// 修改对象属性
        /// </summary>
        public void ModifyObjectAttribute(string objectId, string attributeName, object value)
        {
            var obj = _repository.GetObject(objectId);
            obj.SetAttribute(attributeName, value);
        }

        /// <summary>
        /// 查询对象
        /// </summary>
        public IEnumerable<SemiObject> QueryObjects(Func<SemiObject, bool> predicate)
        {
            var allObjects = _repository.GetAllObjects();
            var result = new List<SemiObject>();
            
            foreach (var obj in allObjects)
            {
                if (predicate(obj))
                {
                    result.Add(obj);
                }
            }
            
            return result;
        }

        /// <summary>
        /// 按类型获取对象
        /// </summary>
        public IEnumerable<SemiObject> GetObjectsByType(string objectType)
        {
            return _repository.GetObjectsByType(objectType);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void PublishEvent(SemiEventArgs eventArgs)
        {
            EventOccurred?.Invoke(this, eventArgs);
        }

        private void OnRepositoryEvent(object sender, ObjectEventArgs e)
        {
            PublishEvent(e);
        }

        /// <summary>
        /// 获取服务统计信息
        /// </summary>
        public string GetStatistics()
        {
            return $"Object Service [{ServiceName}]\n" +
                   $"Total Objects: {_repository.Count()}\n" +
                   $"Service Time: {DateTime.Now}";
        }
    }
}
