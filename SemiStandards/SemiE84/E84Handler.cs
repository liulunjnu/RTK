using System;
using SemiStandards.Common;

namespace SemiE84
{
    /// <summary>
    /// E84处理器 - 管理E84握手协议
    /// </summary>
    public class E84Handler
    {
        private E84StateMachine _stateMachine;
        private string _portId;

        public string PortId
        {
            get { return _portId; }
        }

        public E84State CurrentState
        {
            get { return _stateMachine.State; }
        }

        public E84Signals Signals
        {
            get { return _stateMachine.Signals; }
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<SemiEventArgs> TransferStarted;
        public event EventHandler<SemiEventArgs> TransferCompleted;
        public event EventHandler<SemiEventArgs> TransferAborted;

        public E84Handler(string portId, E84Mode mode)
        {
            _portId = portId;
            _stateMachine = new E84StateMachine(mode, TransferDirection.LoadToEquipment);

            // 订阅状态机事件
            _stateMachine.StateChanged += OnStateMachineStateChanged;
            _stateMachine.TransferCompleted += OnStateMachineTransferCompleted;
            _stateMachine.TimeoutOccurred += OnStateMachineTimeout;
        }

        /// <summary>
        /// 开始装载传输
        /// </summary>
        public void StartLoad()
        {
            _stateMachine.StartLoadTransfer();
            TransferStarted?.Invoke(this, new SemiEventArgs("LoadTransferStarted"));
            LogEvent("Load transfer started");
        }

        /// <summary>
        /// 开始卸载传输
        /// </summary>
        public void StartUnload()
        {
            _stateMachine.StartUnloadTransfer();
            TransferStarted?.Invoke(this, new SemiEventArgs("UnloadTransferStarted"));
            LogEvent("Unload transfer started");
        }

        /// <summary>
        /// 更新输入信号
        /// </summary>
        public void UpdateInputs(E84Signals inputSignals)
        {
            _stateMachine.UpdateInputSignals(inputSignals);
        }

        /// <summary>
        /// 中止传输
        /// </summary>
        public void Abort(string reason)
        {
            _stateMachine.Abort(reason);
            TransferAborted?.Invoke(this, new SemiEventArgs($"TransferAborted: {reason}"));
            LogEvent($"Transfer aborted: {reason}");
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _stateMachine.Reset();
            LogEvent("E84 handler reset");
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public string GetStatus()
        {
            return $"E84 Handler [{_portId}]\n" + _stateMachine.GetStatus();
        }

        private void OnStateMachineStateChanged(object sender, StateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
            LogEvent($"State: {e.PreviousState} -> {e.CurrentState}");
        }

        private void OnStateMachineTransferCompleted(object sender, SemiEventArgs e)
        {
            TransferCompleted?.Invoke(this, e);
            LogEvent("Transfer completed successfully");
        }

        private void OnStateMachineTimeout(object sender, SemiEventArgs e)
        {
            TransferAborted?.Invoke(this, e);
            LogEvent($"Timeout: {e.EventName}");
        }

        private void LogEvent(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] E84[{_portId}] {message}");
        }
    }
}
