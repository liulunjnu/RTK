using System;
using System.Collections.Generic;
using System.Diagnostics;
using SemiStandards.Common;

namespace SemiE116
{
    /// <summary>
    /// E116性能跟踪器
    /// </summary>
    public class PerformanceTracker
    {
        private PerformanceMetrics _currentMetrics;
        private List<PerformanceSnapshot> _history;
        private Stopwatch _operationalTimer;
        private Stopwatch _productiveTimer;
        private DateTime _lastSnapshotTime;

        public PerformanceMetrics CurrentMetrics
        {
            get { return _currentMetrics; }
        }

        public event EventHandler<SemiEventArgs> MetricsUpdated;

        public PerformanceTracker()
        {
            _currentMetrics = new PerformanceMetrics();
            _history = new List<PerformanceSnapshot>();
            _operationalTimer = new Stopwatch();
            _productiveTimer = new Stopwatch();
            _lastSnapshotTime = DateTime.Now;
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        public void StartOperational()
        {
            if (!_operationalTimer.IsRunning)
            {
                _operationalTimer.Start();
            }
        }

        /// <summary>
        /// 停止运行（非计划停机）
        /// </summary>
        public void StopOperational(bool isScheduled = false)
        {
            if (_operationalTimer.IsRunning)
            {
                _operationalTimer.Stop();
                _currentMetrics.OperationalTime += _operationalTimer.Elapsed.TotalSeconds;
                _operationalTimer.Reset();
            }
        }

        /// <summary>
        /// 开始生产
        /// </summary>
        public void StartProductive()
        {
            if (!_productiveTimer.IsRunning)
            {
                _productiveTimer.Start();
            }
        }

        /// <summary>
        /// 停止生产
        /// </summary>
        public void StopProductive()
        {
            if (_productiveTimer.IsRunning)
            {
                _productiveTimer.Stop();
                _currentMetrics.ProductiveTime += _productiveTimer.Elapsed.TotalSeconds;
                _productiveTimer.Reset();
            }
        }

        /// <summary>
        /// 记录停机时间
        /// </summary>
        public void RecordDowntime(double seconds, bool isScheduled)
        {
            if (isScheduled)
            {
                _currentMetrics.ScheduledDownTime += seconds;
            }
            else
            {
                _currentMetrics.UnscheduledDownTime += seconds;
            }
        }

        /// <summary>
        /// 记录基板处理
        /// </summary>
        public void RecordSubstrateProcessed(bool isSuccess)
        {
            _currentMetrics.TotalSubstratesProcessed++;
            
            if (isSuccess)
            {
                _currentMetrics.SuccessfulSubstrates++;
            }
            else
            {
                _currentMetrics.FailedSubstrates++;
            }

            MetricsUpdated?.Invoke(this, new SemiEventArgs("SubstrateProcessed"));
        }

        /// <summary>
        /// 创建性能快照
        /// </summary>
        public PerformanceSnapshot TakeSnapshot()
        {
            // 更新运行时间
            if (_operationalTimer.IsRunning)
            {
                _currentMetrics.OperationalTime += _operationalTimer.Elapsed.TotalSeconds;
                _operationalTimer.Restart();
            }

            if (_productiveTimer.IsRunning)
            {
                _currentMetrics.ProductiveTime += _productiveTimer.Elapsed.TotalSeconds;
                _productiveTimer.Restart();
            }

            var snapshot = new PerformanceSnapshot
            {
                Timestamp = DateTime.Now,
                Metrics = CloneMetrics(_currentMetrics)
            };

            _history.Add(snapshot);
            _lastSnapshotTime = DateTime.Now;

            return snapshot;
        }

        /// <summary>
        /// 获取历史快照
        /// </summary>
        public List<PerformanceSnapshot> GetHistory(DateTime? startTime = null)
        {
            if (startTime.HasValue)
            {
                var result = new List<PerformanceSnapshot>();
                foreach (var snapshot in _history)
                {
                    if (snapshot.Timestamp >= startTime.Value)
                    {
                        result.Add(snapshot);
                    }
                }
                return result;
            }
            
            return new List<PerformanceSnapshot>(_history);
        }

        /// <summary>
        /// 重置指标
        /// </summary>
        public void Reset()
        {
            _currentMetrics.Reset();
            _operationalTimer.Reset();
            _productiveTimer.Reset();
            _history.Clear();
            _lastSnapshotTime = DateTime.Now;
        }

        /// <summary>
        /// 生成性能报告
        /// </summary>
        public string GenerateReport()
        {
            var snapshot = TakeSnapshot();
            
            return $"=== Performance Report ===\n" +
                   $"Report Time: {snapshot.Timestamp:yyyy-MM-dd HH:mm:ss}\n\n" +
                   snapshot.Metrics.ToString() + "\n\n" +
                   $"Time Breakdown:\n" +
                   $"  Operational: {_currentMetrics.OperationalTime:F0} sec\n" +
                   $"  Productive: {_currentMetrics.ProductiveTime:F0} sec\n" +
                   $"  Scheduled Downtime: {_currentMetrics.ScheduledDownTime:F0} sec\n" +
                   $"  Unscheduled Downtime: {_currentMetrics.UnscheduledDownTime:F0} sec\n" +
                   $"  Standby: {_currentMetrics.StandbyTime:F0} sec";
        }

        private PerformanceMetrics CloneMetrics(PerformanceMetrics metrics)
        {
            return new PerformanceMetrics
            {
                OperationalTime = metrics.OperationalTime,
                ScheduledDownTime = metrics.ScheduledDownTime,
                UnscheduledDownTime = metrics.UnscheduledDownTime,
                StandbyTime = metrics.StandbyTime,
                ProductiveTime = metrics.ProductiveTime,
                EngineeringTime = metrics.EngineeringTime,
                TotalSubstratesProcessed = metrics.TotalSubstratesProcessed,
                SuccessfulSubstrates = metrics.SuccessfulSubstrates,
                FailedSubstrates = metrics.FailedSubstrates
            };
        }
    }

    /// <summary>
    /// 性能快照
    /// </summary>
    public class PerformanceSnapshot
    {
        public DateTime Timestamp { get; set; }
        public PerformanceMetrics Metrics { get; set; }

        public override string ToString()
        {
            return $"Snapshot at {Timestamp:HH:mm:ss} - OEE: {Metrics.OEE:F2}%, " +
                   $"Throughput: {Metrics.Throughput:F2}/hr";
        }
    }
}
