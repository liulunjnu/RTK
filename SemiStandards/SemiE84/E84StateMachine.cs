using System;
using System.Threading;
using SemiStandards.Common;

namespace SemiE84
{
    /// <summary>
    /// E84状态机 - 实现完整的E84握手协议
    /// </summary>
    public class E84StateMachine : IStateMachine
    {
        private E84State _currentState;
        private E84Signals _signals;
        private E84TimeoutConfig _timeouts;
        private E84Mode _mode;
        private TransferDirection _direction;
        private DateTime _stateEntryTime;
        private Timer _timeoutTimer;

        public string CurrentState
        {
            get { return _currentState.ToString(); }
        }

        public E84State State
        {
            get { return _currentState; }
        }

        public E84Mode Mode
        {
            get { return _mode; }
        }

        public E84Signals Signals
        {
            get { return _signals; }
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<SemiEventArgs> TimeoutOccurred;
        public event EventHandler<SemiEventArgs> TransferCompleted;

        public E84StateMachine(E84Mode mode, TransferDirection direction)
        {
            _mode = mode;
            _direction = direction;
            _signals = new E84Signals();
            _timeouts = new E84TimeoutConfig();
            _currentState = E84State.ReadyToTransfer;
            _stateEntryTime = DateTime.Now;
        }

        /// <summary>
        /// 状态转换
        /// </summary>
        public bool TransitionTo(string newState)
        {
            if (Enum.TryParse(newState, out E84State targetState))
            {
                return TransitionTo(targetState);
            }
            return false;
        }

        private bool TransitionTo(E84State newState)
        {
            if (_currentState == newState)
                return true;

            var previousState = _currentState;
            _currentState = newState;
            _stateEntryTime = DateTime.Now;

            OnStateChanged(previousState.ToString(), newState.ToString());
            
            return true;
        }

        /// <summary>
        /// 开始装载传输（主动模式）
        /// </summary>
        public void StartLoadTransfer()
        {
            if (_mode != E84Mode.Active)
                throw new InvalidOperationException("Only Active mode can start transfer");

            if (_currentState != E84State.ReadyToTransfer)
                throw new InvalidOperationException($"Cannot start transfer from state {_currentState}");

            _direction = TransferDirection.LoadToEquipment;
            _signals.L_REQ = true;
            TransitionTo(E84State.TransferRequested);
        }

        /// <summary>
        /// 开始卸载传输（主动模式）
        /// </summary>
        public void StartUnloadTransfer()
        {
            if (_mode != E84Mode.Active)
                throw new InvalidOperationException("Only Active mode can start transfer");

            if (_currentState != E84State.ReadyToTransfer)
                throw new InvalidOperationException($"Cannot start transfer from state {_currentState}");

            _direction = TransferDirection.UnloadFromEquipment;
            _signals.U_REQ = true;
            TransitionTo(E84State.TransferRequested);
        }

        /// <summary>
        /// 更新输入信号并推进状态机
        /// </summary>
        public void UpdateInputSignals(E84Signals inputSignals)
        {
            // 检查急停
            if (inputSignals.ES)
            {
                Abort("Emergency Stop activated");
                return;
            }

            // 更新输入信号
            if (_mode == E84Mode.Active)
            {
                _signals.VALID = inputSignals.VALID;
                _signals.CS_0 = inputSignals.CS_0;
                _signals.CS_1 = inputSignals.CS_1;
                _signals.TR_REQ = inputSignals.TR_REQ;
                _signals.HO_AVBL = inputSignals.HO_AVBL;
                _signals.ES = inputSignals.ES;
            }
            else
            {
                _signals.L_REQ = inputSignals.L_REQ;
                _signals.U_REQ = inputSignals.U_REQ;
                _signals.READY = inputSignals.READY;
                _signals.BUSY = inputSignals.BUSY;
                _signals.COMPT = inputSignals.COMPT;
                _signals.CONT = inputSignals.CONT;
            }

            // 推进状态机
            ProcessStateMachine();
        }

        private void ProcessStateMachine()
        {
            switch (_currentState)
            {
                case E84State.ReadyToTransfer:
                    ProcessReadyState();
                    break;

                case E84State.TransferRequested:
                    ProcessRequestedState();
                    break;

                case E84State.TransferReady:
                    ProcessTransferReadyState();
                    break;

                case E84State.Transferring:
                    ProcessTransferringState();
                    break;

                case E84State.TransferComplete:
                    ProcessCompleteState();
                    break;
            }
        }

        private void ProcessReadyState()
        {
            if (_mode == E84Mode.Passive)
            {
                // 被动模式等待L_REQ或U_REQ
                if (_signals.L_REQ || _signals.U_REQ)
                {
                    _direction = _signals.L_REQ ? 
                        TransferDirection.LoadToEquipment : 
                        TransferDirection.UnloadFromEquipment;
                    
                    TransitionTo(E84State.TransferRequested);
                }
            }
        }

        private void ProcessRequestedState()
        {
            if (_mode == E84Mode.Active)
            {
                // 等待VALID信号（TP1超时）
                if (_signals.VALID)
                {
                    _signals.READY = true;
                    TransitionTo(E84State.TransferReady);
                }
                else if (GetStateTime() > _timeouts.TP1_ValidSignal)
                {
                    OnTimeout(E84Timeout.TP1_ValidSignal);
                    Abort("TP1 Timeout: VALID signal not received");
                }
            }
            else // Passive
            {
                // 被动模式设置VALID和TR_REQ
                _signals.VALID = true;
                _signals.TR_REQ = true;
                
                // 等待READY信号
                if (_signals.READY)
                {
                    TransitionTo(E84State.TransferReady);
                }
            }
        }

        private void ProcessTransferReadyState()
        {
            if (_mode == E84Mode.Active)
            {
                // 等待TR_REQ信号
                if (_signals.TR_REQ)
                {
                    _signals.BUSY = true;
                    _signals.READY = false;
                    TransitionTo(E84State.Transferring);
                }
            }
            else // Passive
            {
                // 等待BUSY信号
                if (_signals.BUSY && !_signals.READY)
                {
                    // 开始物理传输
                    TransitionTo(E84State.Transferring);
                }
            }
        }

        private void ProcessTransferringState()
        {
            if (_mode == E84Mode.Active)
            {
                // 等待CS_0信号（载体固定）
                if (_signals.CS_0)
                {
                    _signals.COMPT = true;
                    _signals.BUSY = false;
                    TransitionTo(E84State.TransferComplete);
                }
                else if (GetStateTime() > _timeouts.TP3_BusySignal)
                {
                    OnTimeout(E84Timeout.TP3_BusySignal);
                    Abort("TP3 Timeout: Transfer not completed");
                }
            }
            else // Passive
            {
                // 模拟物理传输完成
                Thread.Sleep(100);
                _signals.CS_0 = true;
                
                // 等待COMPT信号
                if (_signals.COMPT && !_signals.BUSY)
                {
                    TransitionTo(E84State.TransferComplete);
                }
            }
        }

        private void ProcessCompleteState()
        {
            if (_mode == E84Mode.Active)
            {
                // 等待CONT信号，然后复位
                _signals.CONT = true;
                _signals.L_REQ = false;
                _signals.U_REQ = false;
                _signals.COMPT = false;
                
                Thread.Sleep(100);
                
                TransitionTo(E84State.ReadyToTransfer);
                OnTransferCompleted();
            }
            else // Passive
            {
                // 等待所有信号复位
                if (_signals.CONT)
                {
                    _signals.VALID = false;
                    _signals.TR_REQ = false;
                    _signals.CS_0 = false;
                    
                    TransitionTo(E84State.ReadyToTransfer);
                    OnTransferCompleted();
                }
            }
        }

        /// <summary>
        /// 中止传输
        /// </summary>
        public void Abort(string reason)
        {
            _signals.Reset();
            TransitionTo(E84State.TransferAborted);
            
            var eventArgs = new SemiEventArgs("TransferAborted");
            TimeoutOccurred?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// 重置状态机
        /// </summary>
        public void Reset()
        {
            _signals.Reset();
            TransitionTo(E84State.ReadyToTransfer);
        }

        public System.Collections.Generic.IEnumerable<string> GetValidTransitions()
        {
            var transitions = new System.Collections.Generic.List<string>();
            
            switch (_currentState)
            {
                case E84State.ReadyToTransfer:
                    transitions.Add(E84State.TransferRequested.ToString());
                    break;
                case E84State.TransferRequested:
                    transitions.Add(E84State.TransferReady.ToString());
                    transitions.Add(E84State.TransferAborted.ToString());
                    break;
                case E84State.TransferReady:
                    transitions.Add(E84State.Transferring.ToString());
                    transitions.Add(E84State.TransferAborted.ToString());
                    break;
                case E84State.Transferring:
                    transitions.Add(E84State.TransferComplete.ToString());
                    transitions.Add(E84State.TransferAborted.ToString());
                    break;
                case E84State.TransferComplete:
                    transitions.Add(E84State.ReadyToTransfer.ToString());
                    break;
            }
            
            return transitions;
        }

        private int GetStateTime()
        {
            return (int)(DateTime.Now - _stateEntryTime).TotalMilliseconds;
        }

        private void OnStateChanged(string previousState, string currentState)
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(previousState, currentState));
        }

        private void OnTimeout(E84Timeout timeoutType)
        {
            var eventArgs = new SemiEventArgs($"Timeout_{timeoutType}");
            TimeoutOccurred?.Invoke(this, eventArgs);
        }

        private void OnTransferCompleted()
        {
            TransferCompleted?.Invoke(this, new SemiEventArgs("TransferCompleted"));
        }

        public string GetStatus()
        {
            return $"E84 State Machine [{_mode}]\n" +
                   $"State: {_currentState}\n" +
                   $"Direction: {_direction}\n" +
                   $"Signals: {_signals.GetSignalStatus()}\n" +
                   $"State Time: {GetStateTime()}ms";
        }
    }
}
