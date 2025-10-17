using System;
using System.Collections.Generic;
using Gem300.Framework.Common;

namespace Gem300.Framework.E94
{
    public enum ControlJobState { Created, Started, Completed, Aborted }

    public sealed class ControlJob
    {
        public string ControlJobId { get; set; }
        public ControlJobState State { get; set; }
        public List<CarrierId> BoundCarriers { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }

        public ControlJob()
        {
            State = ControlJobState.Created;
            CreatedAtUtc = DateTime.UtcNow;
            BoundCarriers = new List<CarrierId>();
        }
    }

    public sealed class ControlJobManager
    {
        private readonly Dictionary<string, ControlJob> _jobs = new Dictionary<string, ControlJob>();

        public ResultOf<ControlJob> Create(string id)
        {
            if (_jobs.ContainsKey(id)) return ResultOf<ControlJob>.Fail("Already exists");
            var cj = new ControlJob { ControlJobId = id };
            _jobs.Add(id, cj);
            return ResultOf<ControlJob>.Ok(cj);
        }

        public Result BindCarrier(string id, CarrierId carrier)
        {
            ControlJob cj; if (!_jobs.TryGetValue(id, out cj)) return Result.Fail("CJ not found");
            if (cj.State != ControlJobState.Created) return Result.Fail("CJ not in Created");
            cj.BoundCarriers.Add(carrier);
            return Result.Ok();
        }

        public Result Start(string id)
        {
            ControlJob cj; if (!_jobs.TryGetValue(id, out cj)) return Result.Fail("CJ not found");
            if (cj.State != ControlJobState.Created) return Result.Fail("Invalid state");
            cj.State = ControlJobState.Started;
            cj.StartedAtUtc = DateTime.UtcNow;
            return Result.Ok();
        }

        public Result Complete(string id)
        {
            ControlJob cj; if (!_jobs.TryGetValue(id, out cj)) return Result.Fail("CJ not found");
            if (cj.State != ControlJobState.Started) return Result.Fail("Invalid state");
            cj.State = ControlJobState.Completed;
            cj.FinishedAtUtc = DateTime.UtcNow;
            return Result.Ok();
        }

        public Result Abort(string id, string reason)
        {
            ControlJob cj; if (!_jobs.TryGetValue(id, out cj)) return Result.Fail("CJ not found");
            cj.State = ControlJobState.Aborted;
            cj.FinishedAtUtc = DateTime.UtcNow;
            return Result.Ok();
        }

        public ResultOf<ControlJob> Get(string id)
        {
            ControlJob cj; return _jobs.TryGetValue(id, out cj) ? ResultOf<ControlJob>.Ok(cj) : ResultOf<ControlJob>.Fail("Not found");
        }
    }
}
