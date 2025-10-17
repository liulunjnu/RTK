using System;
using System.Collections.Generic;
using System.Linq;
using SemiStandards.Common;

namespace SemiE94
{
    /// <summary>
    /// E94控制作业管理器
    /// </summary>
    public class ControlJobManager
    {
        private Dictionary<string, ControlJob> _controlJobs;
        
        public event EventHandler<SemiEventArgs> ControlJobCreated;
        public event EventHandler<SemiEventArgs> ControlJobStateChanged;
        public event EventHandler<SemiEventArgs> ControlJobCompleted;

        public ControlJobManager()
        {
            _controlJobs = new Dictionary<string, ControlJob>();
        }

        /// <summary>
        /// 创建控制作业
        /// </summary>
        public ControlJob CreateControlJob(string controlJobId, string processRecipe, 
            List<string> carrierIds, Dictionary<string, List<string>> substratesByCarrier)
        {
            if (_controlJobs.ContainsKey(controlJobId))
                throw new InvalidOperationException($"ControlJob {controlJobId} already exists");

            var controlJob = new ControlJob(controlJobId, processRecipe);

            // 为每个载体创建CarrierJob
            foreach (var carrierId in carrierIds)
            {
                var carrierJobId = $"{controlJobId}_{carrierId}";
                var carrierJob = new CarrierJob(carrierJobId, carrierId);

                // 为载体中的每个基板创建ProcessJob
                if (substratesByCarrier.ContainsKey(carrierId))
                {
                    var substrates = substratesByCarrier[carrierId];
                    for (int i = 0; i < substrates.Count; i++)
                    {
                        var processJobId = $"{carrierJobId}_{i + 1}";
                        var processJob = new ProcessJob(processJobId, substrates[i], processRecipe)
                        {
                            SlotNumber = i + 1
                        };
                        carrierJob.AddProcessJob(processJob);
                    }
                }

                controlJob.AddCarrierJob(carrierJob);
            }

            _controlJobs[controlJobId] = controlJob;
            ControlJobCreated?.Invoke(this, new SemiEventArgs($"ControlJobCreated:{controlJobId}"));

            return controlJob;
        }

        /// <summary>
        /// 开始控制作业
        /// </summary>
        public void StartControlJob(string controlJobId)
        {
            var controlJob = GetControlJob(controlJobId);
            
            if (controlJob.State != ControlJobState.Queued && 
                controlJob.State != ControlJobState.Selected)
            {
                throw new InvalidOperationException($"Cannot start ControlJob in state {controlJob.State}");
            }

            controlJob.State = ControlJobState.Executing;
            controlJob.StartTime = DateTime.Now;

            // 开始所有载体作业
            foreach (var carrierJob in controlJob.GetCarrierJobs())
            {
                carrierJob.State = CarrierJobState.Processing;
            }

            ControlJobStateChanged?.Invoke(this, 
                new SemiEventArgs($"ControlJobStarted:{controlJobId}"));
        }

        /// <summary>
        /// 开始处理基板
        /// </summary>
        public void StartProcessJob(string controlJobId, string substrateId, string stationId)
        {
            var controlJob = GetControlJob(controlJobId);
            
            foreach (var carrierJob in controlJob.GetCarrierJobs())
            {
                var processJob = carrierJob.ProcessJobs.FirstOrDefault(pj => pj.SubstrateId == substrateId);
                if (processJob != null)
                {
                    processJob.Start(stationId);
                    return;
                }
            }
        }

        /// <summary>
        /// 完成处理基板
        /// </summary>
        public void CompleteProcessJob(string controlJobId, string substrateId, string resultCode)
        {
            var controlJob = GetControlJob(controlJobId);
            
            foreach (var carrierJob in controlJob.GetCarrierJobs())
            {
                var processJob = carrierJob.ProcessJobs.FirstOrDefault(pj => pj.SubstrateId == substrateId);
                if (processJob != null)
                {
                    processJob.Complete(resultCode);
                    
                    // 检查载体作业是否完成
                    if (carrierJob.ProcessJobs.All(pj => 
                        pj.State == ProcessJobState.ProcessingComplete || 
                        pj.State == ProcessJobState.Skipped))
                    {
                        carrierJob.State = CarrierJobState.Complete;
                    }
                    
                    // 检查控制作业是否完成
                    CheckControlJobCompletion(controlJob);
                    return;
                }
            }
        }

        /// <summary>
        /// 暂停控制作业
        /// </summary>
        public void PauseControlJob(string controlJobId)
        {
            var controlJob = GetControlJob(controlJobId);
            controlJob.State = ControlJobState.Paused;
            
            foreach (var carrierJob in controlJob.GetCarrierJobs())
            {
                if (carrierJob.State == CarrierJobState.Processing)
                {
                    carrierJob.State = CarrierJobState.Paused;
                }
            }

            ControlJobStateChanged?.Invoke(this, 
                new SemiEventArgs($"ControlJobPaused:{controlJobId}"));
        }

        /// <summary>
        /// 恢复控制作业
        /// </summary>
        public void ResumeControlJob(string controlJobId)
        {
            var controlJob = GetControlJob(controlJobId);
            controlJob.State = ControlJobState.Executing;
            
            foreach (var carrierJob in controlJob.GetCarrierJobs())
            {
                if (carrierJob.State == CarrierJobState.Paused)
                {
                    carrierJob.State = CarrierJobState.Processing;
                }
            }

            ControlJobStateChanged?.Invoke(this, 
                new SemiEventArgs($"ControlJobResumed:{controlJobId}"));
        }

        /// <summary>
        /// 取消控制作业
        /// </summary>
        public void CancelControlJob(string controlJobId)
        {
            var controlJob = GetControlJob(controlJobId);
            controlJob.State = ControlJobState.Canceled;
            controlJob.CompletedTime = DateTime.Now;

            ControlJobCompleted?.Invoke(this, 
                new SemiEventArgs($"ControlJobCanceled:{controlJobId}"));
        }

        private void CheckControlJobCompletion(ControlJob controlJob)
        {
            if (controlJob.GetCarrierJobs().All(cj => cj.State == CarrierJobState.Complete))
            {
                controlJob.State = ControlJobState.Completed;
                controlJob.CompletedTime = DateTime.Now;
                
                ControlJobCompleted?.Invoke(this, 
                    new SemiEventArgs($"ControlJobCompleted:{controlJob.ControlJobId}"));
            }
        }

        public ControlJob GetControlJob(string controlJobId)
        {
            if (!_controlJobs.ContainsKey(controlJobId))
                throw new KeyNotFoundException($"ControlJob {controlJobId} not found");

            return _controlJobs[controlJobId];
        }

        public List<ControlJob> GetAllControlJobs()
        {
            return _controlJobs.Values.ToList();
        }

        public List<ControlJob> GetActiveControlJobs()
        {
            return _controlJobs.Values
                .Where(cj => cj.State == ControlJobState.Executing)
                .ToList();
        }
    }
}
