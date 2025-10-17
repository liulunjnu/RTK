using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Gem300.Common;

namespace Gem300.E39;

public interface IE39Object
{
    string ObjectType { get; }
    string ObjectId { get; }
    IReadOnlyDictionary<string, object?> GetAttributes();
    Result SetAttribute(string name, object? value);
}

public sealed class E39Registry
{
    private readonly ConcurrentDictionary<(string type, string id), IE39Object> objects = new();

    public Result Register(IE39Object obj)
    {
        if (!objects.TryAdd((obj.ObjectType, obj.ObjectId), obj))
            return Result.Fail($"Object already exists: {obj.ObjectType}:{obj.ObjectId}");
        return Result.Ok();
    }

    public Result Unregister(string type, string id)
    {
        return objects.TryRemove((type, id), out _) ? Result.Ok() : Result.Fail("Object not found");
    }

    public Result<IE39Object> Get(string type, string id)
    {
        return objects.TryGetValue((type, id), out var obj)
            ? Result<IE39Object>.Ok(obj)
            : Result<IE39Object>.Fail("Object not found");
    }

    public IReadOnlyCollection<IE39Object> ListByType(string type)
    {
        var list = new List<IE39Object>();
        foreach (var kvp in objects)
        {
            if (kvp.Key.type == type) list.Add(kvp.Value);
        }
        return list;
    }
}
