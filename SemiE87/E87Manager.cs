using System;
using System.Collections.Generic;
using System.Linq;

namespace SemiE87
{
    /// <summary>
    /// E87载体管理器 (E87 Carrier Manager)
    /// SEMI E87协议的主要管理类，负责协调载体和装载端口
    /// </summary>
    public class E87Manager
    {
        /// <summary>装载端口集合</summary>
        private Dictionary<string, LoadPort> _loadPorts;

        /// <summary>载体集合</summary>
        private Dictionary<string, Carrier> _carriers;

        /// <summary>设备ID</summary>
        public string EquipmentId { get; private set; }

        /// <summary>是否在线</summary>
        public bool IsOnline { get; set; }

        // 事件定义
        public event EventHandler<CarrierEventArgs> CarrierRegistered;
        public event EventHandler<CarrierEventArgs> CarrierUnregistered;
        public event EventHandler<CarrierEventArgs> CarrierIdRead;
        public event EventHandler<CarrierEventArgs> CarrierIdVerified;
        public event EventHandler<LoadPortEventArgs> LoadPortStateChanged;

        public E87Manager(string equipmentId)
        {
            EquipmentId = equipmentId;
            _loadPorts = new Dictionary<string, LoadPort>();
            _carriers = new Dictionary<string, Carrier>();
            IsOnline = false;
        }

        #region 装载端口管理

        /// <summary>
        /// 添加装载端口
        /// </summary>
        public void AddLoadPort(string loadPortId, string location = "")
        {
            if (_loadPorts.ContainsKey(loadPortId))
            {
                throw new ArgumentException($"LoadPort {loadPortId} already exists");
            }

            var loadPort = new LoadPort(loadPortId, location);
            
            // 订阅装载端口事件
            loadPort.StateChanged += OnLoadPortStateChanged;
            loadPort.CarrierArrived += OnCarrierArrived;
            loadPort.CarrierDeparted += OnCarrierDeparted;
            loadPort.TransferStateChanged += OnLoadPortTransferStateChanged;
            loadPort.DoorStateChanged += OnLoadPortDoorStateChanged;

            _loadPorts.Add(loadPortId, loadPort);
        }

        /// <summary>
        /// 获取装载端口
        /// </summary>
        public LoadPort GetLoadPort(string loadPortId)
        {
            if (_loadPorts.TryGetValue(loadPortId, out var loadPort))
            {
                return loadPort;
            }
            throw new KeyNotFoundException($"LoadPort {loadPortId} not found");
        }

        /// <summary>
        /// 获取所有装载端口
        /// </summary>
        public List<LoadPort> GetAllLoadPorts()
        {
            return _loadPorts.Values.ToList();
        }

        /// <summary>
        /// 获取可用的装载端口（空闲且准备就绪）
        /// </summary>
        public List<LoadPort> GetAvailableLoadPorts()
        {
            return _loadPorts.Values
                .Where(lp => lp.IsEnabled && lp.State == LoadPortState.Empty)
                .ToList();
        }

        #endregion

        #region 载体管理

        /// <summary>
        /// 注册载体
        /// </summary>
        public Carrier RegisterCarrier(string carrierId, int slotCount = 25, string carrierType = "FOUP")
        {
            if (_carriers.ContainsKey(carrierId))
            {
                return _carriers[carrierId];
            }

            var carrier = new Carrier(carrierId, slotCount)
            {
                CarrierType = carrierType
            };

            _carriers.Add(carrierId, carrier);
            CarrierRegistered?.Invoke(this, new CarrierEventArgs(carrier));

            return carrier;
        }

        /// <summary>
        /// 注销载体
        /// </summary>
        public bool UnregisterCarrier(string carrierId)
        {
            if (_carriers.TryGetValue(carrierId, out var carrier))
            {
                // 如果载体还在使用中，先断开关联
                if (carrier.AssociationState != CarrierAssociationState.NotAssociated)
                {
                    carrier.Disassociate();
                }

                _carriers.Remove(carrierId);
                CarrierUnregistered?.Invoke(this, new CarrierEventArgs(carrier));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取载体
        /// </summary>
        public Carrier GetCarrier(string carrierId)
        {
            if (_carriers.TryGetValue(carrierId, out var carrier))
            {
                return carrier;
            }
            return null;
        }

        /// <summary>
        /// 获取所有载体
        /// </summary>
        public List<Carrier> GetAllCarriers()
        {
            return _carriers.Values.ToList();
        }

        /// <summary>
        /// 读取载体ID（模拟RFID或条码读取）
        /// </summary>
        public bool ReadCarrierId(string loadPortId, string carrierId)
        {
            var loadPort = GetLoadPort(loadPortId);
            var carrier = RegisterCarrier(carrierId);

            carrier.IdStatus = CarrierIdStatus.IdRead;
            CarrierIdRead?.Invoke(this, new CarrierEventArgs(carrier));

            return true;
        }

        /// <summary>
        /// 验证载体ID
        /// </summary>
        public bool VerifyCarrierId(string carrierId, string expectedId)
        {
            var carrier = GetCarrier(carrierId);
            if (carrier == null)
            {
                return false;
            }

            bool isValid = carrier.ValidateCarrierId(expectedId);
            if (isValid)
            {
                CarrierIdVerified?.Invoke(this, new CarrierEventArgs(carrier));
            }

            return isValid;
        }

        #endregion

        #region 载体与装载端口操作

        /// <summary>
        /// 将载体装载到指定的装载端口
        /// </summary>
        public bool LoadCarrierToPort(string carrierId, string loadPortId)
        {
            var carrier = GetCarrier(carrierId);
            if (carrier == null)
            {
                throw new ArgumentException($"Carrier {carrierId} not found");
            }

            var loadPort = GetLoadPort(loadPortId);
            
            // 执行装载操作
            return loadPort.LoadCarrier(carrier);
        }

        /// <summary>
        /// 从装载端口卸载载体
        /// </summary>
        public bool UnloadCarrierFromPort(string loadPortId)
        {
            var loadPort = GetLoadPort(loadPortId);
            return loadPort.UnloadCarrier();
        }

        /// <summary>
        /// 打开装载端口的门
        /// </summary>
        public bool OpenLoadPortDoor(string loadPortId)
        {
            var loadPort = GetLoadPort(loadPortId);
            return loadPort.OpenDoor();
        }

        /// <summary>
        /// 关闭装载端口的门
        /// </summary>
        public bool CloseLoadPortDoor(string loadPortId)
        {
            var loadPort = GetLoadPort(loadPortId);
            return loadPort.CloseDoor();
        }

        /// <summary>
        /// 执行载体的槽位映射（读取载体内晶圆的位置）
        /// </summary>
        public bool PerformSlotMapping(string carrierId, Dictionary<int, SlotOccupancyStatus> slotStatuses)
        {
            var carrier = GetCarrier(carrierId);
            if (carrier == null)
            {
                return false;
            }

            carrier.UpdateSlotMap(slotStatuses);
            return true;
        }

        /// <summary>
        /// 设置载体访问模式
        /// </summary>
        public bool SetCarrierAccessMode(string carrierId, CarrierAccessMode accessMode)
        {
            var carrier = GetCarrier(carrierId);
            if (carrier == null)
            {
                return false;
            }

            carrier.AccessMode = accessMode;
            carrier.LastUpdatedTime = DateTime.Now;
            return true;
        }

        #endregion

        #region 事件处理

        private void OnLoadPortStateChanged(object sender, LoadPortEventArgs e)
        {
            LoadPortStateChanged?.Invoke(this, e);
            LogEvent($"LoadPort {e.LoadPortId} state changed at {e.EventTime}");
        }

        private void OnCarrierArrived(object sender, LoadPortEventArgs e)
        {
            LogEvent($"Carrier {e.Carrier?.CarrierId} arrived at LoadPort {e.LoadPortId}");
        }

        private void OnCarrierDeparted(object sender, LoadPortEventArgs e)
        {
            LogEvent($"Carrier {e.Carrier?.CarrierId} departed from LoadPort {e.LoadPortId}");
        }

        private void OnLoadPortTransferStateChanged(object sender, LoadPortEventArgs e)
        {
            LogEvent($"LoadPort {e.LoadPortId} transfer state changed");
        }

        private void OnLoadPortDoorStateChanged(object sender, LoadPortEventArgs e)
        {
            var loadPort = sender as LoadPort;
            LogEvent($"LoadPort {e.LoadPortId} door state: {loadPort?.DoorState}");
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 获取指定装载端口的载体
        /// </summary>
        public Carrier GetCarrierAtLoadPort(string loadPortId)
        {
            var loadPort = GetLoadPort(loadPortId);
            return loadPort.AssociatedCarrier;
        }

        /// <summary>
        /// 查找载体所在的装载端口
        /// </summary>
        public LoadPort FindLoadPortByCarrier(string carrierId)
        {
            return _loadPorts.Values.FirstOrDefault(
                lp => lp.AssociatedCarrier?.CarrierId == carrierId);
        }

        /// <summary>
        /// 获取系统状态摘要
        /// </summary>
        public string GetSystemStatus()
        {
            var summary = $"=== E87 System Status (Equipment: {EquipmentId}) ===\n";
            summary += $"Online: {IsOnline}\n";
            summary += $"Total LoadPorts: {_loadPorts.Count}\n";
            summary += $"Total Carriers: {_carriers.Count}\n\n";

            summary += "LoadPorts:\n";
            foreach (var lp in _loadPorts.Values)
            {
                summary += $"  {lp}\n";
            }

            summary += "\nCarriers:\n";
            foreach (var carrier in _carriers.Values)
            {
                summary += $"  {carrier}\n";
            }

            return summary;
        }

        #endregion

        #region 日志

        private void LogEvent(string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
        }

        #endregion
    }

    /// <summary>
    /// 载体事件参数
    /// </summary>
    public class CarrierEventArgs : EventArgs
    {
        public Carrier Carrier { get; set; }
        public DateTime EventTime { get; set; }

        public CarrierEventArgs(Carrier carrier)
        {
            Carrier = carrier;
            EventTime = DateTime.Now;
        }
    }
}
