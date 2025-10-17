using System;

namespace SemiE84
{
    /// <summary>
    /// E84信号集合 - 并行I/O信号
    /// </summary>
    public class E84Signals
    {
        // ===== 主动端(Active)输出信号 =====
        
        /// <summary>L_REQ - Load Request (装载请求)</summary>
        public bool L_REQ { get; set; }
        
        /// <summary>U_REQ - Unload Request (卸载请求)</summary>
        public bool U_REQ { get; set; }
        
        /// <summary>READY - Ready to Transfer (准备传输)</summary>
        public bool READY { get; set; }
        
        /// <summary>BUSY - Transfer in Progress (传输中)</summary>
        public bool BUSY { get; set; }
        
        /// <summary>COMPT - Transfer Complete (传输完成)</summary>
        public bool COMPT { get; set; }
        
        /// <summary>CONT - Continue (继续)</summary>
        public bool CONT { get; set; }

        // ===== 主动端(Active)输入信号 =====
        
        /// <summary>VALID - Carrier Present (载体在位)</summary>
        public bool VALID { get; set; }
        
        /// <summary>CS_0 - Carrier Secure 0 (载体固定0)</summary>
        public bool CS_0 { get; set; }
        
        /// <summary>CS_1 - Carrier Secure 1 (载体固定1)</summary>
        public bool CS_1 { get; set; }
        
        /// <summary>TR_REQ - Transfer Request (传输请求)</summary>
        public bool TR_REQ { get; set; }
        
        /// <summary>HO_AVBL - Handoff Available (交接可用)</summary>
        public bool HO_AVBL { get; set; }
        
        /// <summary>ES - Emergency Stop (急停)</summary>
        public bool ES { get; set; }

        // ===== 被动端(Passive)的信号是相反的 =====
        
        public E84Signals()
        {
            Reset();
        }

        /// <summary>
        /// 重置所有信号为初始状态
        /// </summary>
        public void Reset()
        {
            L_REQ = false;
            U_REQ = false;
            READY = false;
            BUSY = false;
            COMPT = false;
            CONT = false;
            VALID = false;
            CS_0 = false;
            CS_1 = false;
            TR_REQ = false;
            HO_AVBL = false;
            ES = false;
        }

        /// <summary>
        /// 获取当前信号状态字符串
        /// </summary>
        public string GetSignalStatus()
        {
            return $"L_REQ={B(L_REQ)} U_REQ={B(U_REQ)} READY={B(READY)} BUSY={B(BUSY)} " +
                   $"COMPT={B(COMPT)} CONT={B(CONT)} | " +
                   $"VALID={B(VALID)} CS_0={B(CS_0)} CS_1={B(CS_1)} " +
                   $"TR_REQ={B(TR_REQ)} HO_AVBL={B(HO_AVBL)} ES={B(ES)}";
        }

        private string B(bool value)
        {
            return value ? "1" : "0";
        }

        /// <summary>
        /// 克隆信号状态
        /// </summary>
        public E84Signals Clone()
        {
            return new E84Signals
            {
                L_REQ = this.L_REQ,
                U_REQ = this.U_REQ,
                READY = this.READY,
                BUSY = this.BUSY,
                COMPT = this.COMPT,
                CONT = this.CONT,
                VALID = this.VALID,
                CS_0 = this.CS_0,
                CS_1 = this.CS_1,
                TR_REQ = this.TR_REQ,
                HO_AVBL = this.HO_AVBL,
                ES = this.ES
            };
        }
    }

    /// <summary>
    /// E84超时配置
    /// </summary>
    public class E84TimeoutConfig
    {
        /// <summary>TP1 - VALID信号超时 (毫秒)</summary>
        public int TP1_ValidSignal { get; set; }
        
        /// <summary>TP2 - CS_0信号超时 (毫秒)</summary>
        public int TP2_CS0Signal { get; set; }
        
        /// <summary>TP3 - BUSY信号超时 (毫秒)</summary>
        public int TP3_BusySignal { get; set; }
        
        /// <summary>TP4 - COMPT信号超时 (毫秒)</summary>
        public int TP4_ComptSignal { get; set; }
        
        /// <summary>TP5 - CONT信号超时 (毫秒)</summary>
        public int TP5_ContSignal { get; set; }

        public E84TimeoutConfig()
        {
            // 默认超时值（SEMI E84标准）
            TP1_ValidSignal = 8000;      // 8秒
            TP2_CS0Signal = 8000;        // 8秒
            TP3_BusySignal = 120000;     // 120秒
            TP4_ComptSignal = 8000;      // 8秒
            TP5_ContSignal = 8000;       // 8秒
        }
    }
}
