using System;

namespace SemiE116
{
    /// <summary>
    /// 设备性能指标 - SEMI E116
    /// </summary>
    public class PerformanceMetrics
    {
        // ===== E10计算指标 =====
        
        /// <summary>运行时间 (秒)</summary>
        public double OperationalTime { get; set; }
        
        /// <summary>计划停机时间 (秒)</summary>
        public double ScheduledDownTime { get; set; }
        
        /// <summary>非计划停机时间 (秒)</summary>
        public double UnscheduledDownTime { get; set; }
        
        /// <summary>待机时间 (秒)</summary>
        public double StandbyTime { get; set; }
        
        /// <summary>生产时间 (秒)</summary>
        public double ProductiveTime { get; set; }
        
        /// <summary>工程时间 (秒)</summary>
        public double EngineeringTime { get; set; }

        // ===== 产量指标 =====
        
        /// <summary>处理的基板总数</summary>
        public int TotalSubstratesProcessed { get; set; }
        
        /// <summary>成功处理的基板数</summary>
        public int SuccessfulSubstrates { get; set; }
        
        /// <summary>失败的基板数</summary>
        public int FailedSubstrates { get; set; }

        // ===== 计算属性 =====
        
        /// <summary>
        /// 可用度 (Availability) = 运行时间 / (运行时间 + 停机时间)
        /// </summary>
        public double Availability
        {
            get
            {
                var totalTime = OperationalTime + UnscheduledDownTime;
                return totalTime > 0 ? (OperationalTime / totalTime) * 100 : 0;
            }
        }

        /// <summary>
        /// 性能效率 (Performance) = 理论周期时间 / 实际周期时间
        /// </summary>
        public double Performance
        {
            get
            {
                if (TotalSubstratesProcessed == 0 || ProductiveTime == 0)
                    return 0;
                
                // 假设理论周期时间为60秒
                var idealCycleTime = 60.0;
                var actualCycleTime = ProductiveTime / TotalSubstratesProcessed;
                
                return Math.Min((idealCycleTime / actualCycleTime) * 100, 100);
            }
        }

        /// <summary>
        /// 质量率 (Quality) = 成功基板数 / 总基板数
        /// </summary>
        public double Quality
        {
            get
            {
                return TotalSubstratesProcessed > 0 ? 
                    ((double)SuccessfulSubstrates / TotalSubstratesProcessed) * 100 : 0;
            }
        }

        /// <summary>
        /// OEE (Overall Equipment Effectiveness) = A × P × Q
        /// </summary>
        public double OEE
        {
            get
            {
                return (Availability / 100) * (Performance / 100) * (Quality / 100) * 100;
            }
        }

        /// <summary>
        /// MTBF (Mean Time Between Failures) - 平均故障间隔时间
        /// </summary>
        public double MTBF
        {
            get
            {
                var failureCount = Math.Max(1, FailedSubstrates);
                return OperationalTime / failureCount;
            }
        }

        /// <summary>
        /// MTTR (Mean Time To Repair) - 平均修复时间
        /// </summary>
        public double MTTR
        {
            get
            {
                var repairEvents = Math.Max(1, (int)(UnscheduledDownTime / 3600)); // 假设每小时一个故障
                return UnscheduledDownTime / repairEvents;
            }
        }

        /// <summary>
        /// 吞吐量 (基板/小时)
        /// </summary>
        public double Throughput
        {
            get
            {
                var hours = OperationalTime / 3600;
                return hours > 0 ? TotalSubstratesProcessed / hours : 0;
            }
        }

        public PerformanceMetrics()
        {
            Reset();
        }

        public void Reset()
        {
            OperationalTime = 0;
            ScheduledDownTime = 0;
            UnscheduledDownTime = 0;
            StandbyTime = 0;
            ProductiveTime = 0;
            EngineeringTime = 0;
            TotalSubstratesProcessed = 0;
            SuccessfulSubstrates = 0;
            FailedSubstrates = 0;
        }

        public override string ToString()
        {
            return $"Performance Metrics:\n" +
                   $"  OEE: {OEE:F2}%\n" +
                   $"  - Availability: {Availability:F2}%\n" +
                   $"  - Performance: {Performance:F2}%\n" +
                   $"  - Quality: {Quality:F2}%\n" +
                   $"  Throughput: {Throughput:F2} substrates/hour\n" +
                   $"  MTBF: {MTBF:F0} sec, MTTR: {MTTR:F0} sec\n" +
                   $"  Processed: {TotalSubstratesProcessed} (Success: {SuccessfulSubstrates}, Failed: {FailedSubstrates})";
        }
    }
}
