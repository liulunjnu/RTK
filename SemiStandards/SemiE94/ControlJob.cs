using System;
using System.Collections.Generic;
using System.Linq;

namespace SemiE94
{
    /// <summary>
    /// 控制作业 - SEMI E94
    /// 管理一组载体的处理作业
    /// </summary>
    public class ControlJob
    {
        public string ControlJobId { get; set; }
        public ControlJobState State { get; set; }
        public string ProcessRecipe { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompletedTime { get; set; }
        
        private List<CarrierJob> _carrierJobs;

        public ControlJob(string controlJobId, string processRecipe)
        {
            ControlJobId = controlJobId;
            ProcessRecipe = processRecipe;
            State = ControlJobState.Queued;
            CreatedTime = DateTime.Now;
            _carrierJobs = new List<CarrierJob>();
        }

        public void AddCarrierJob(CarrierJob carrierJob)
        {
            _carrierJobs.Add(carrierJob);
        }

        public List<CarrierJob> GetCarrierJobs()
        {
            return new List<CarrierJob>(_carrierJobs);
        }

        public int GetTotalSubstrateCount()
        {
            return _carrierJobs.Sum(cj => cj.ProcessJobs.Count);
        }

        public int GetCompletedSubstrateCount()
        {
            return _carrierJobs.Sum(cj => 
                cj.ProcessJobs.Count(pj => pj.State == ProcessJobState.ProcessingComplete));
        }

        public override string ToString()
        {
            return $"ControlJob[{ControlJobId}] State={State}, Recipe={ProcessRecipe}, " +
                   $"Carriers={_carrierJobs.Count}, Progress={GetCompletedSubstrateCount()}/{GetTotalSubstrateCount()}";
        }
    }

    /// <summary>
    /// 载体作业
    /// </summary>
    public class CarrierJob
    {
        public string CarrierJobId { get; set; }
        public string CarrierId { get; set; }
        public CarrierJobState State { get; set; }
        public string LoadPortId { get; set; }
        
        public List<ProcessJob> ProcessJobs { get; private set; }

        public CarrierJob(string carrierJobId, string carrierId)
        {
            CarrierJobId = carrierJobId;
            CarrierId = carrierId;
            State = CarrierJobState.WaitingForStart;
            ProcessJobs = new List<ProcessJob>();
        }

        public void AddProcessJob(ProcessJob processJob)
        {
            ProcessJobs.Add(processJob);
        }

        public override string ToString()
        {
            return $"CarrierJob[{CarrierJobId}] Carrier={CarrierId}, State={State}, Jobs={ProcessJobs.Count}";
        }
    }
}
