using System;
using System.Collections.Generic;
using System.Linq;
using SemiStandards.Common;

namespace SemiE39
{
    /// <summary>
    /// 对象仓库 - 管理所有SEMI对象
    /// </summary>
    public class ObjectRepository
    {
        private Dictionary<string, SemiObject> _objects;
        
        public event EventHandler<ObjectEventArgs> ObjectCreated;
        public event EventHandler<ObjectEventArgs> ObjectDeleted;

        public ObjectRepository()
        {
            _objects = new Dictionary<string, SemiObject>();
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        public SemiObject CreateObject(string objectId, string objectType)
        {
            if (_objects.ContainsKey(objectId))
                throw new InvalidOperationException($"Object {objectId} already exists");

            var obj = new SemiObject(objectId, objectType);
            _objects[objectId] = obj;

            ObjectCreated?.Invoke(this, new ObjectEventArgs(objectId, objectType, ObjectEventType.Created));
            
            return obj;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public SemiObject GetObject(string objectId)
        {
            if (!_objects.ContainsKey(objectId))
                throw new KeyNotFoundException($"Object {objectId} not found");

            return _objects[objectId];
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public bool DeleteObject(string objectId)
        {
            if (!_objects.ContainsKey(objectId))
                return false;

            var obj = _objects[objectId];
            _objects.Remove(objectId);

            ObjectDeleted?.Invoke(this, new ObjectEventArgs(objectId, obj.ObjectType, ObjectEventType.Deleted));
            
            return true;
        }

        /// <summary>
        /// 检查对象是否存在
        /// </summary>
        public bool Exists(string objectId)
        {
            return _objects.ContainsKey(objectId);
        }

        /// <summary>
        /// 获取所有对象
        /// </summary>
        public IEnumerable<SemiObject> GetAllObjects()
        {
            return _objects.Values;
        }

        /// <summary>
        /// 按类型查询对象
        /// </summary>
        public IEnumerable<SemiObject> GetObjectsByType(string objectType)
        {
            return _objects.Values.Where(obj => obj.ObjectType == objectType);
        }

        /// <summary>
        /// 获取对象数量
        /// </summary>
        public int Count()
        {
            return _objects.Count;
        }

        /// <summary>
        /// 清空所有对象
        /// </summary>
        public void Clear()
        {
            _objects.Clear();
        }
    }
}
