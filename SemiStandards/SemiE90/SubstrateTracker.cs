using System;
using System.Collections.Generic;
using System.Linq;
using SemiStandards.Common;

namespace SemiE90
{
    /// <summary>
    /// E90基板跟踪管理器
    /// </summary>
    public class SubstrateTracker
    {
        private Dictionary<string, Substrate> _substrates;
        
        public event EventHandler<SemiEventArgs> SubstrateRegistered;
        public event EventHandler<SemiEventArgs> SubstrateStateChanged;
        public event EventHandler<SemiEventArgs> SubstrateProcessed;

        public SubstrateTracker()
        {
            _substrates = new Dictionary<string, Substrate>();
        }

        /// <summary>
        /// 注册基板
        /// </summary>
        public Substrate RegisterSubstrate(string substrateId, SubstrateType type = SubstrateType.ProductWafer)
        {
            if (_substrates.ContainsKey(substrateId))
                return _substrates[substrateId];

            var substrate = new Substrate(substrateId)
            {
                Type = type,
                State = SubstrateState.InCarrier
            };

            _substrates[substrateId] = substrate;
            SubstrateRegistered?.Invoke(this, new SemiEventArgs($"SubstrateRegistered:{substrateId}"));

            return substrate;
        }

        /// <summary>
        /// 关联基板到载体
        /// </summary>
        public void AssociateWithCarrier(string substrateId, string carrierId, int slotNumber)
        {
            var substrate = GetSubstrate(substrateId);
            substrate.CarrierId = carrierId;
            substrate.SlotNumber = slotNumber;
            substrate.UpdateState(SubstrateState.InCarrier, $"Carrier:{carrierId}:Slot{slotNumber}");
        }

        /// <summary>
        /// 更新基板位置
        /// </summary>
        public void UpdateLocation(string substrateId, SubstrateState state, string location, string stationId = null)
        {
            var substrate = GetSubstrate(substrateId);
            substrate.UpdateState(state, location);
            substrate.CurrentStationId = stationId;

            SubstrateStateChanged?.Invoke(this, new SemiEventArgs($"StateChanged:{substrateId}:{state}"));
        }

        /// <summary>
        /// 开始处理
        /// </summary>
        public void StartProcessing(string substrateId, string stationId, string recipeId)
        {
            var substrate = GetSubstrate(substrateId);
            substrate.State = SubstrateState.Processing;
            substrate.CurrentStationId = stationId;
            substrate.RecipeId = recipeId;
            substrate.StartProcessTime = DateTime.Now;

            substrate.AddHistory(new SubstrateHistoryEntry
            {
                Timestamp = DateTime.Now,
                Event = "ProcessStarted",
                StationId = stationId,
                Details = $"Recipe: {recipeId}"
            });
        }

        /// <summary>
        /// 完成处理
        /// </summary>
        public void CompleteProcessing(string substrateId, SubstrateProcessResult result)
        {
            var substrate = GetSubstrate(substrateId);
            substrate.State = SubstrateState.Completed;
            substrate.ProcessResult = result;
            substrate.EndProcessTime = DateTime.Now;

            substrate.AddHistory(new SubstrateHistoryEntry
            {
                Timestamp = DateTime.Now,
                Event = "ProcessCompleted",
                Details = $"Result: {result}"
            });

            SubstrateProcessed?.Invoke(this, new SemiEventArgs($"ProcessCompleted:{substrateId}:{result}"));
        }

        /// <summary>
        /// 获取基板
        /// </summary>
        public Substrate GetSubstrate(string substrateId)
        {
            if (!_substrates.ContainsKey(substrateId))
                throw new KeyNotFoundException($"Substrate {substrateId} not found");

            return _substrates[substrateId];
        }

        /// <summary>
        /// 获取载体中的所有基板
        /// </summary>
        public List<Substrate> GetSubstratesByCarrier(string carrierId)
        {
            return _substrates.Values
                .Where(s => s.CarrierId == carrierId)
                .OrderBy(s => s.SlotNumber)
                .ToList();
        }

        /// <summary>
        /// 获取处理统计
        /// </summary>
        public Dictionary<string, int> GetStatistics()
        {
            return new Dictionary<string, int>
            {
                { "Total", _substrates.Count },
                { "InCarrier", _substrates.Values.Count(s => s.State == SubstrateState.InCarrier) },
                { "Processing", _substrates.Values.Count(s => s.State == SubstrateState.Processing) },
                { "Completed", _substrates.Values.Count(s => s.State == SubstrateState.Completed) },
                { "Success", _substrates.Values.Count(s => s.ProcessResult == SubstrateProcessResult.Success) },
                { "Failed", _substrates.Values.Count(s => s.ProcessResult == SubstrateProcessResult.Failed) }
            };
        }
    }
}
