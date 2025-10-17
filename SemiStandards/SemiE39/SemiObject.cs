using System;
using System.Collections.Generic;
using SemiStandards.Common;

namespace SemiE39
{
    /// <summary>
    /// SEMI E39 标准对象实现
    /// 提供对象的属性管理、版本控制和事件通知
    /// </summary>
    public class SemiObject : ISemiObject
    {
        private Dictionary<string, ObjectAttribute> _attributes;
        
        public string ObjectId { get; private set; }
        public string ObjectType { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public DateTime ModifiedTime { get; private set; }
        public int Version { get; private set; }

        public event EventHandler<ObjectEventArgs> ObjectModified;

        public SemiObject(string objectId, string objectType)
        {
            if (string.IsNullOrEmpty(objectId))
                throw new ArgumentException("ObjectId cannot be null or empty");
            if (string.IsNullOrEmpty(objectType))
                throw new ArgumentException("ObjectType cannot be null or empty");

            ObjectId = objectId;
            ObjectType = objectType;
            CreatedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            Version = 1;
            _attributes = new Dictionary<string, ObjectAttribute>();

            // 添加标准属性
            AddAttribute("ObjectId", objectId, AttributeAccess.ReadOnly);
            AddAttribute("ObjectType", objectType, AttributeAccess.ReadOnly);
            AddAttribute("CreatedTime", CreatedTime, AttributeAccess.ReadOnly);
            AddAttribute("ModifiedTime", ModifiedTime, AttributeAccess.ReadOnly);
            AddAttribute("Version", Version, AttributeAccess.ReadOnly);
        }

        /// <summary>
        /// 添加属性定义
        /// </summary>
        public void AddAttribute(string name, object value, AttributeAccess access = AttributeAccess.ReadWrite)
        {
            if (_attributes.ContainsKey(name))
                throw new InvalidOperationException($"Attribute {name} already exists");

            _attributes[name] = new ObjectAttribute
            {
                Name = name,
                Value = value,
                Access = access,
                DataType = value?.GetType().Name ?? "Unknown"
            };
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public object GetAttribute(string attributeName)
        {
            if (!_attributes.ContainsKey(attributeName))
                throw new KeyNotFoundException($"Attribute {attributeName} not found");

            return _attributes[attributeName].Value;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public void SetAttribute(string attributeName, object value)
        {
            if (!_attributes.ContainsKey(attributeName))
                throw new KeyNotFoundException($"Attribute {attributeName} not found");

            var attribute = _attributes[attributeName];
            
            if (attribute.Access == AttributeAccess.ReadOnly)
                throw new InvalidOperationException($"Attribute {attributeName} is read-only");

            var oldValue = attribute.Value;
            attribute.Value = value;
            
            ModifiedTime = DateTime.Now;
            Version++;
            
            // 更新系统属性
            _attributes["ModifiedTime"].Value = ModifiedTime;
            _attributes["Version"].Value = Version;

            OnObjectModified();
        }

        /// <summary>
        /// 获取所有属性
        /// </summary>
        public IDictionary<string, object> GetAllAttributes()
        {
            var result = new Dictionary<string, object>();
            foreach (var attr in _attributes)
            {
                result[attr.Key] = attr.Value.Value;
            }
            return result;
        }

        /// <summary>
        /// 获取属性定义
        /// </summary>
        public ObjectAttribute GetAttributeDefinition(string attributeName)
        {
            if (!_attributes.ContainsKey(attributeName))
                throw new KeyNotFoundException($"Attribute {attributeName} not found");

            return _attributes[attributeName];
        }

        /// <summary>
        /// 获取所有属性定义
        /// </summary>
        public IEnumerable<ObjectAttribute> GetAllAttributeDefinitions()
        {
            return _attributes.Values;
        }

        /// <summary>
        /// 检查属性是否存在
        /// </summary>
        public bool HasAttribute(string attributeName)
        {
            return _attributes.ContainsKey(attributeName);
        }

        protected virtual void OnObjectModified()
        {
            ObjectModified?.Invoke(this, new ObjectEventArgs(ObjectId, ObjectType, ObjectEventType.Modified));
        }

        public override string ToString()
        {
            return $"SemiObject[{ObjectType}:{ObjectId}] v{Version}";
        }
    }

    /// <summary>
    /// 对象属性定义
    /// </summary>
    public class ObjectAttribute
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string DataType { get; set; }
        public AttributeAccess Access { get; set; }
        public string Description { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public string Units { get; set; }

        public override string ToString()
        {
            return $"{Name} ({DataType}): {Value} [{Access}]";
        }
    }
}
