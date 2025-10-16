using System;
using System.Collections.Generic;
using System.Linq;

namespace SemiE87
{
    /// <summary>
    /// 载体对象 (Carrier Object) - SEMI E87 规范
    /// 代表一个FOUP/SMIF载体
    /// </summary>
    public class Carrier
    {
        /// <summary>载体ID</summary>
        public string CarrierId { get; set; }

        /// <summary>载体ID状态</summary>
        public CarrierIdStatus IdStatus { get; set; }

        /// <summary>载体位置</summary>
        public CarrierLocation Location { get; set; }

        /// <summary>载体关联状态</summary>
        public CarrierAssociationState AssociationState { get; set; }

        /// <summary>访问模式</summary>
        public CarrierAccessMode AccessMode { get; set; }

        /// <summary>关联的装载端口ID</summary>
        public string AssociatedLoadPortId { get; set; }

        /// <summary>载体槽位数量（通常是25或13）</summary>
        public int SlotCount { get; private set; }

        /// <summary>槽位映射表</summary>
        public List<CarrierSlot> SlotMap { get; private set; }

        /// <summary>槽位映射状态</summary>
        public SlotMapStatus SlotMapStatus { get; set; }

        /// <summary>载体类型 (例如: FOUP, FOSB)</summary>
        public string CarrierType { get; set; }

        /// <summary>载体创建时间</summary>
        public DateTime CreatedTime { get; private set; }

        /// <summary>最后更新时间</summary>
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>载体使用状态</summary>
        public bool IsInUse { get; set; }

        public Carrier(string carrierId, int slotCount = 25)
        {
            CarrierId = carrierId;
            SlotCount = slotCount;
            IdStatus = CarrierIdStatus.NotRead;
            Location = CarrierLocation.Unknown;
            AssociationState = CarrierAssociationState.NotAssociated;
            AccessMode = CarrierAccessMode.Unknown;
            SlotMapStatus = SlotMapStatus.NotVerified;
            CarrierType = "FOUP";
            CreatedTime = DateTime.Now;
            LastUpdatedTime = DateTime.Now;
            IsInUse = false;

            // 初始化槽位映射
            SlotMap = new List<CarrierSlot>();
            for (int i = 1; i <= slotCount; i++)
            {
                SlotMap.Add(new CarrierSlot(i));
            }
        }

        /// <summary>
        /// 关联到装载端口
        /// </summary>
        public void AssociateWithLoadPort(string loadPortId)
        {
            AssociatedLoadPortId = loadPortId;
            AssociationState = CarrierAssociationState.AssociatedWithLoadPort;
            Location = CarrierLocation.AtLoadPort;
            LastUpdatedTime = DateTime.Now;
        }

        /// <summary>
        /// 取消关联
        /// </summary>
        public void Disassociate()
        {
            AssociatedLoadPortId = string.Empty;
            AssociationState = CarrierAssociationState.NotAssociated;
            Location = CarrierLocation.NotAtEquipment;
            LastUpdatedTime = DateTime.Now;
        }

        /// <summary>
        /// 更新槽位映射
        /// </summary>
        public void UpdateSlotMap(Dictionary<int, SlotOccupancyStatus> slotStatuses)
        {
            foreach (var kvp in slotStatuses)
            {
                var slot = SlotMap.FirstOrDefault(s => s.SlotNumber == kvp.Key);
                if (slot != null)
                {
                    slot.OccupancyStatus = kvp.Value;
                }
            }
            SlotMapStatus = SlotMapStatus.Verified;
            LastUpdatedTime = DateTime.Now;
        }

        /// <summary>
        /// 获取已占用的槽位数量
        /// </summary>
        public int GetOccupiedSlotCount()
        {
            return SlotMap.Count(s => s.OccupancyStatus == SlotOccupancyStatus.Occupied);
        }

        /// <summary>
        /// 验证载体ID
        /// </summary>
        public bool ValidateCarrierId(string expectedId)
        {
            if (CarrierId == expectedId)
            {
                IdStatus = CarrierIdStatus.IdVerified;
                LastUpdatedTime = DateTime.Now;
                return true;
            }
            else
            {
                IdStatus = CarrierIdStatus.IdVerificationFailed;
                LastUpdatedTime = DateTime.Now;
                return false;
            }
        }

        public override string ToString()
        {
            return $"Carrier[ID={CarrierId}, Location={Location}, " +
                   $"State={AssociationState}, Slots={GetOccupiedSlotCount()}/{SlotCount}]";
        }
    }
}
