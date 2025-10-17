using System;
using System.Collections.Generic;
using Gem300.Common;

namespace Gem300.E94;

public enum ControlJobState { Created, Started, Completed, Aborted }

public sealed class ControlJob
{
    public required string ControlJobId { get; init; }
    public ControlJobState State { get; set; } = ControlJobState.Created;
    public List<CarrierId> BoundCarriers { get; } = new();
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }
}

public sealed class ControlJobManager
{
    private readonly Dictionary<string, ControlJob> jobs = new();

    public Result<ControlJob> Create(string id)
    {
        if (jobs.ContainsKey(id)) return Result<ControlJob>.Fail("Already exists");
        var cj = new ControlJob { ControlJobId = id };
        jobs.Add(id, cj);
        return Result<ControlJob>.Ok(cj);
    }

    public Result BindCarrier(string id, CarrierId carrier)
    {
        if (!jobs.TryGetValue(id, out var cj)) return Result.Fail("CJ not found");
        if (cj.State != ControlJobState.Created) return Result.Fail("CJ not in Created");
        cj.BoundCarriers.Add(carrier);
        return Result.Ok();
    }

    public Result Start(string id)
    {
        if (!jobs.TryGetValue(id, out var cj)) return Result.Fail("CJ not found");
        if (cj.State != ControlJobState.Created) return Result.Fail("Invalid state");
        cj.State = ControlJobState.Started;
        cj.StartedAtUtc = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result Complete(string id)
    {
        if (!jobs.TryGetValue(id, out var cj)) return Result.Fail("CJ not found");
        if (cj.State != ControlJobState.Started) return Result.Fail("Invalid state");
        cj.State = ControlJobState.Completed;
        cj.FinishedAtUtc = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result Abort(string id, string reason)
    {
        if (!jobs.TryGetValue(id, out var cj)) return Result.Fail("CJ not found");
        cj.State = ControlJobState.Aborted;
        cj.FinishedAtUtc = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result<ControlJob> Get(string id)
    {
        return jobs.TryGetValue(id, out var cj) ? Result<ControlJob>.Ok(cj) : Result<ControlJob>.Fail("Not found");
    }
}
