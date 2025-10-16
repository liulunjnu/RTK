using System;

namespace SemiE87
{
    /// <summary>
    /// 装载端口 (Load Port) - SEMI E87 规范
    /// 代表设备上的一个载体装载位置
    /// </summary>
    public class LoadPort
    {
        /// <summary>装载端口ID</summary>
        public string LoadPortId { get; set; }

        /// <summary>装载端口状态</summary>
        public LoadPortState State { get; set; }

        /// <summary>传输状态</summary>
        public LoadPortTransferState TransferState { get; set; }

        /// <summary>当前关联的载体</summary>
        public Carrier AssociatedCarrier { get; set; }

        /// <summary>夹紧状态</summary>
        public ClampState ClampState { get; set; }

        /// <summary>门状态</summary>
        public DoorState DoorState { get; set; }

        /// <summary>是否有载体在位</summary>
        public bool IsCarrierPresent => AssociatedCarrier != null;

        /// <summary>是否准备好接收载体</summary>
        public bool IsReadyToReceive => State == LoadPortState.Empty && 
                                        TransferState == LoadPortTransferState.ReadyToLoad;

        /// <summary>是否准备好释放载体</summary>
        public bool IsReadyToRelease => State == LoadPortState.Loaded && 
                                        TransferState == LoadPortTransferState.ReadyToUnload &&
                                        DoorState == DoorState.Closed;

        /// <summary>装载端口位置信息（可选）</summary>
        public string Location { get; set; }

        /// <summary>是否启用</summary>
        public bool IsEnabled { get; set; }

        /// <summary>最后状态变更时间</summary>
        public DateTime LastStateChangeTime { get; set; }

        // 事件定义
        public event EventHandler<LoadPortEventArgs> StateChanged;
        public event EventHandler<LoadPortEventArgs> CarrierArrived;
        public event EventHandler<LoadPortEventArgs> CarrierDeparted;
        public event EventHandler<LoadPortEventArgs> TransferStateChanged;
        public event EventHandler<LoadPortEventArgs> DoorStateChanged;
        public event EventHandler<LoadPortEventArgs> ClampStateChanged;

        public LoadPort(string loadPortId, string location = "")
        {
            LoadPortId = loadPortId;
            Location = location;
            State = LoadPortState.Empty;
            TransferState = LoadPortTransferState.None;
            ClampState = ClampState.Unclamped;
            DoorState = DoorState.Closed;
            IsEnabled = true;
            LastStateChangeTime = DateTime.Now;
        }

        /// <summary>
        /// 装载载体到端口
        /// </summary>
        public bool LoadCarrier(Carrier carrier)
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException($"LoadPort {LoadPortId} is disabled");
            }

            if (State != LoadPortState.Empty)
            {
                throw new InvalidOperationException(
                    $"LoadPort {LoadPortId} is not empty. Current state: {State}");
            }

            // 开始装载过程
            SetState(LoadPortState.Loading);
            SetTransferState(LoadPortTransferState.Transferring);

            // 关联载体
            AssociatedCarrier = carrier;
            carrier.AssociateWithLoadPort(LoadPortId);

            // 夹紧载体
            SetClampState(ClampState.Clamped);

            // 完成装载
            SetState(LoadPortState.Loaded);
            SetTransferState(LoadPortTransferState.TransferComplete);

            CarrierArrived?.Invoke(this, new LoadPortEventArgs(LoadPortId, carrier));

            return true;
        }

        /// <summary>
        /// 从端口卸载载体
        /// </summary>
        public bool UnloadCarrier()
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException($"LoadPort {LoadPortId} is disabled");
            }

            if (AssociatedCarrier == null)
            {
                throw new InvalidOperationException($"No carrier at LoadPort {LoadPortId}");
            }

            if (DoorState != DoorState.Closed)
            {
                throw new InvalidOperationException(
                    $"Cannot unload: Door is {DoorState}. Must be closed first.");
            }

            var carrier = AssociatedCarrier;

            // 开始卸载过程
            SetState(LoadPortState.Unloading);
            SetTransferState(LoadPortTransferState.Transferring);

            // 松开夹紧
            SetClampState(ClampState.Unclamped);

            // 取消关联
            carrier.Disassociate();
            AssociatedCarrier = null;

            // 完成卸载
            SetState(LoadPortState.Empty);
            SetTransferState(LoadPortTransferState.None);

            CarrierDeparted?.Invoke(this, new LoadPortEventArgs(LoadPortId, carrier));

            return true;
        }

        /// <summary>
        /// 打开门以访问载体内部的晶圆
        /// </summary>
        public bool OpenDoor()
        {
            if (AssociatedCarrier == null)
            {
                throw new InvalidOperationException("No carrier present");
            }

            if (ClampState != ClampState.Clamped)
            {
                throw new InvalidOperationException("Carrier must be clamped before opening door");
            }

            SetDoorState(DoorState.Opening);
            // 模拟开门过程
            System.Threading.Thread.Sleep(100);
            SetDoorState(DoorState.Open);

            return true;
        }

        /// <summary>
        /// 关闭门
        /// </summary>
        public bool CloseDoor()
        {
            if (DoorState == DoorState.Closed)
            {
                return true;
            }

            SetDoorState(DoorState.Closing);
            // 模拟关门过程
            System.Threading.Thread.Sleep(100);
            SetDoorState(DoorState.Closed);

            return true;
        }

        /// <summary>
        /// 设置装载端口状态
        /// </summary>
        private void SetState(LoadPortState newState)
        {
            if (State != newState)
            {
                State = newState;
                LastStateChangeTime = DateTime.Now;
                StateChanged?.Invoke(this, new LoadPortEventArgs(LoadPortId, AssociatedCarrier));
            }
        }

        /// <summary>
        /// 设置传输状态
        /// </summary>
        private void SetTransferState(LoadPortTransferState newState)
        {
            if (TransferState != newState)
            {
                TransferState = newState;
                TransferStateChanged?.Invoke(this, new LoadPortEventArgs(LoadPortId, AssociatedCarrier));
            }
        }

        /// <summary>
        /// 设置门状态
        /// </summary>
        private void SetDoorState(DoorState newState)
        {
            if (DoorState != newState)
            {
                DoorState = newState;
                DoorStateChanged?.Invoke(this, new LoadPortEventArgs(LoadPortId, AssociatedCarrier));
            }
        }

        /// <summary>
        /// 设置夹紧状态
        /// </summary>
        private void SetClampState(ClampState newState)
        {
            if (ClampState != newState)
            {
                ClampState = newState;
                ClampStateChanged?.Invoke(this, new LoadPortEventArgs(LoadPortId, AssociatedCarrier));
            }
        }

        /// <summary>
        /// 准备接收载体
        /// </summary>
        public void PrepareForReceive()
        {
            if (State == LoadPortState.Empty)
            {
                SetTransferState(LoadPortTransferState.ReadyToLoad);
            }
        }

        /// <summary>
        /// 准备释放载体
        /// </summary>
        public void PrepareForRelease()
        {
            if (State == LoadPortState.Loaded && DoorState == DoorState.Closed)
            {
                SetTransferState(LoadPortTransferState.ReadyToUnload);
            }
        }

        public override string ToString()
        {
            return $"LoadPort[{LoadPortId}] State={State}, Transfer={TransferState}, " +
                   $"Door={DoorState}, Carrier={AssociatedCarrier?.CarrierId ?? "None"}";
        }
    }

    /// <summary>
    /// 装载端口事件参数
    /// </summary>
    public class LoadPortEventArgs : EventArgs
    {
        public string LoadPortId { get; set; }
        public Carrier Carrier { get; set; }
        public DateTime EventTime { get; set; }

        public LoadPortEventArgs(string loadPortId, Carrier carrier)
        {
            LoadPortId = loadPortId;
            Carrier = carrier;
            EventTime = DateTime.Now;
        }
    }
}
