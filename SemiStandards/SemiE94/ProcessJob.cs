using System;

namespace SemiE94
{
    /// <summary>
    /// 处理作业 - 单个基板的处理任务
    /// </summary>
    public class ProcessJob
    {
        public string ProcessJobId { get; set; }
        public string SubstrateId { get; set; }
        public ProcessJobState State { get; set; }
        public string RecipeId { get; set; }
        public int SlotNumber { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public string StationId { get; set; }
        public string ResultCode { get; set; }
        public string ErrorMessage { get; set; }

        public ProcessJob(string processJobId, string substrateId, string recipeId)
        {
            ProcessJobId = processJobId;
            SubstrateId = substrateId;
            RecipeId = recipeId;
            State = ProcessJobState.WaitingForProcessing;
        }

        public void Start(string stationId)
        {
            State = ProcessJobState.Processing;
            StationId = stationId;
            StartTime = DateTime.Now;
        }

        public void Complete(string resultCode)
        {
            State = ProcessJobState.ProcessingComplete;
            ResultCode = resultCode;
            EndTime = DateTime.Now;
        }

        public void Abort(string errorMessage)
        {
            State = ProcessJobState.Aborted;
            ErrorMessage = errorMessage;
            EndTime = DateTime.Now;
        }

        public TimeSpan GetProcessDuration()
        {
            if (StartTime == DateTime.MinValue)
                return TimeSpan.Zero;
            
            var endTime = EndTime != DateTime.MinValue ? EndTime : DateTime.Now;
            return endTime - StartTime;
        }

        public override string ToString()
        {
            return $"ProcessJob[{ProcessJobId}] Substrate={SubstrateId}, State={State}, Recipe={RecipeId}";
        }
    }
}
