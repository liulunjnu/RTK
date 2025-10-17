using System.Collections.Concurrent;
using System.Collections.Generic;
using Gem300.Framework.Common;

namespace Gem300.Framework.E39
{
    public interface IE39Object
    {
        string ObjectType { get; }
        string ObjectId { get; }
        IReadOnlyDictionary<string, object> GetAttributes();
        Result SetAttribute(string name, object value);
    }

    public sealed class E39Registry
    {
        private static string Key(string type, string id) { return type + "|" + id; }
        private readonly ConcurrentDictionary<string, IE39Object> _objects = new ConcurrentDictionary<string, IE39Object>();

        public Result Register(IE39Object obj)
        {
            return _objects.TryAdd(Key(obj.ObjectType, obj.ObjectId), obj) ? Result.Ok() : Result.Fail("Already exists");
        }

        public Result Unregister(string type, string id)
        {
            IE39Object _; return _objects.TryRemove(Key(type, id), out _) ? Result.Ok() : Result.Fail("Not found");
        }

        public ResultOf<IE39Object> Get(string type, string id)
        {
            IE39Object obj; return _objects.TryGetValue(Key(type, id), out obj) ? ResultOf<IE39Object>.Ok(obj) : ResultOf<IE39Object>.Fail("Not found");
        }

        public IReadOnlyCollection<IE39Object> ListByType(string type)
        {
            var list = new List<IE39Object>();
            foreach (var kv in _objects)
            {
                if (kv.Key.StartsWith(type + "|")) list.Add(kv.Value);
            }
            return list;
        }
    }
}
