# SEMI Standards Implementation (.NET Framework 4.7.2)

完整的半导体设备SEMI标准套件C#实现，包含E39、E84、E87、E90、E94、E116标准。

## 📋 项目概述

这是一个基于 **.NET Framework 4.7.2** 的完整SEMI标准实现，适用于半导体制造设备的自动化控制和管理。

### 包含的SEMI标准

| 标准 | 名称 | 描述 | 状态 |
|------|------|------|------|
| **E39** | Object Services | 对象服务标准 - 提供统一的对象管理和属性访问接口 | ✅ 完成 |
| **E84** | Enhanced Carrier Handoff | 增强型载体交接 - 并行I/O接口的载体握手协议 | ✅ 完成 |
| **E87** | Carrier Management | 载体管理规范 - FOUP/SMIF载体的管理和跟踪 | ✅ 完成 |
| **E90** | Substrate Tracking | 基板跟踪标准 - 晶圆级别的位置和状态跟踪 | ✅ 完成 |
| **E94** | Control Job Management | 控制作业管理 - 批次作业的创建、执行和监控 | ✅ 完成 |
| **E116** | Equipment Performance Tracking | 设备性能跟踪 - OEE、MTBF、MTTR等性能指标 | ✅ 完成 |

## 🏗️ 解决方案结构

```
SemiStandards/
├── SemiStandards.sln                    # 主解决方案文件
│
├── SemiStandards.Common/                # 公共库
│   ├── CommonEnums.cs                   # 通用枚举定义
│   ├── EventArgs.cs                     # 事件参数类
│   └── Interfaces.cs                    # 公共接口
│
├── SemiE39/                             # E39 对象服务
│   ├── SemiObject.cs                    # SEMI对象实现
│   ├── ObjectRepository.cs              # 对象仓库
│   └── ObjectService.cs                 # 对象服务管理器
│
├── SemiE84/                             # E84 载体交接
│   ├── E84Enums.cs                      # E84枚举
│   ├── E84Signals.cs                    # 并行I/O信号
│   ├── E84StateMachine.cs               # E84状态机
│   └── E84Handler.cs                    # E84处理器
│
├── SemiE87/                             # E87 载体管理
│   ├── E87Enums.cs                      # E87枚举
│   ├── Carrier.cs                       # 载体类
│   ├── LoadPort.cs                      # 装载端口类
│   └── E87Manager.cs                    # E87管理器
│
├── SemiE90/                             # E90 基板跟踪
│   ├── E90Enums.cs                      # E90枚举
│   ├── Substrate.cs                     # 基板对象
│   └── SubstrateTracker.cs              # 基板跟踪器
│
├── SemiE94/                             # E94 控制作业
│   ├── E94Enums.cs                      # E94枚举
│   ├── ControlJob.cs                    # 控制作业类
│   ├── ProcessJob.cs                    # 处理作业类
│   └── ControlJobManager.cs             # 控制作业管理器
│
├── SemiE116/                            # E116 性能跟踪
│   ├── PerformanceMetrics.cs            # 性能指标
│   └── PerformanceTracker.cs            # 性能跟踪器
│
└── SemiStandards.Demo/                  # 集成演示
    └── Program.cs                       # 完整演示程序
```

## 🚀 快速开始

### 环境要求

- **Visual Studio 2017** 或更高版本
- **.NET Framework 4.7.2** 或更高版本
- Windows 7 SP1 / Windows Server 2008 R2 SP1 或更高版本

### 编译项目

```bash
# 使用MSBuild
cd SemiStandards
msbuild SemiStandards.sln /p:Configuration=Release

# 或使用Visual Studio
# 打开 SemiStandards.sln 并按 F6
```

### 运行演示

```bash
cd SemiStandards.Demo\bin\Debug
SemiStandards.Demo.exe
```

## 📖 详细说明

### E39 - 对象服务 (Object Services)

提供统一的对象模型，所有设备实体都可以表示为对象。

**核心功能：**
- ✅ 对象创建、删除、修改
- ✅ 属性管理（只读、读写、只写）
- ✅ 对象查询和过滤
- ✅ 版本控制
- ✅ 事件通知

**使用示例：**

```csharp
var objectService = new ObjectService("MyService");

// 创建对象
var equipment = objectService.CreateObject("EQP-001", "Equipment", 
    new Dictionary<string, object>
    {
        { "Model", "300mm Processor" },
        { "Manufacturer", "SEMI Corp" }
    });

// 修改属性
objectService.ModifyObjectAttribute("EQP-001", "Model", "300mm Processor v2");

// 查询对象
var equipments = objectService.GetObjectsByType("Equipment");
```

### E84 - 载体交接 (Enhanced Carrier Handoff)

实现设备间的载体交接握手协议，基于并行I/O信号。

**核心功能：**
- ✅ 主动/被动模式
- ✅ 完整的E84状态机
- ✅ 12个标准I/O信号（L_REQ, U_REQ, READY, BUSY, COMPT, CONT, VALID, CS_0, CS_1, TR_REQ, HO_AVBL, ES）
- ✅ TP1-TP6超时管理
- ✅ 装载/卸载传输

**使用示例：**

```csharp
var e84Handler = new E84Handler("LP1", E84Mode.Active);

// 订阅事件
e84Handler.StateChanged += (s, e) => 
    Console.WriteLine($"State: {e.PreviousState} -> {e.CurrentState}");

// 开始装载
e84Handler.StartLoad();

// 更新输入信号
var signals = new E84Signals { VALID = true, TR_REQ = true };
e84Handler.UpdateInputs(signals);
```

### E87 - 载体管理 (Carrier Management)

管理FOUP/SMIF载体及装载端口。

**核心功能：**
- ✅ 载体注册和跟踪
- ✅ 载体ID读取和验证（RFID/条码）
- ✅ 装载端口管理
- ✅ 载体位置跟踪
- ✅ 槽位映射（25槽位FOUP）
- ✅ 访问模式控制（自动/手动）

**使用示例：**

```csharp
var e87Manager = new E87Manager("EQP-001");

// 添加装载端口
e87Manager.AddLoadPort("LP1");

// 注册载体
var carrier = e87Manager.RegisterCarrier("CARRIER-001", 25);

// 装载载体
e87Manager.LoadCarrierToPort("CARRIER-001", "LP1");

// 更新槽位映射
var slotMap = new Dictionary<int, SlotOccupancyStatus>
{
    { 1, SlotOccupancyStatus.Occupied },
    { 2, SlotOccupancyStatus.Occupied },
    { 3, SlotOccupancyStatus.Empty }
};
carrier.UpdateSlotMap(slotMap);
```

### E90 - 基板跟踪 (Substrate Tracking)

跟踪单个晶圆（基板）的位置和处理状态。

**核心功能：**
- ✅ 基板注册和跟踪
- ✅ 载体关联
- ✅ 位置更新
- ✅ 处理状态管理
- ✅ 历史记录
- ✅ 统计报告

**使用示例：**

```csharp
var e90Tracker = new SubstrateTracker();

// 注册基板
var substrate = e90Tracker.RegisterSubstrate("WAFER-001");

// 关联到载体
e90Tracker.AssociateWithCarrier("WAFER-001", "CARRIER-001", 1);

// 更新位置
e90Tracker.UpdateLocation("WAFER-001", SubstrateState.AtStation, "Chamber1");

// 开始处理
e90Tracker.StartProcessing("WAFER-001", "Chamber1", "RECIPE-001");

// 完成处理
e90Tracker.CompleteProcessing("WAFER-001", SubstrateProcessResult.Success);

// 获取统计
var stats = e90Tracker.GetStatistics();
Console.WriteLine($"Success: {stats["Success"]}, Failed: {stats["Failed"]}");
```

### E94 - 控制作业管理 (Control Job Management)

管理批次处理作业。

**核心功能：**
- ✅ 控制作业创建
- ✅ 载体作业管理
- ✅ 处理作业调度
- ✅ 作业状态跟踪
- ✅ 暂停/恢复/取消
- ✅ 进度监控

**使用示例：**

```csharp
var e94Manager = new ControlJobManager();

// 创建控制作业
var controlJob = e94Manager.CreateControlJob(
    "CJ-001", 
    "RECIPE-001",
    new List<string> { "CARRIER-001", "CARRIER-002" },
    new Dictionary<string, List<string>>
    {
        { "CARRIER-001", new List<string> { "W1", "W2", "W3" } },
        { "CARRIER-002", new List<string> { "W4", "W5" } }
    });

// 开始作业
e94Manager.StartControlJob("CJ-001");

// 处理单个基板
e94Manager.StartProcessJob("CJ-001", "W1", "Chamber1");
e94Manager.CompleteProcessJob("CJ-001", "W1", "SUCCESS");

// 暂停作业
e94Manager.PauseControlJob("CJ-001");

// 恢复作业
e94Manager.ResumeControlJob("CJ-001");
```

### E116 - 性能跟踪 (Equipment Performance Tracking)

跟踪设备性能指标。

**核心功能：**
- ✅ OEE (Overall Equipment Effectiveness) 计算
- ✅ 可用度 (Availability)
- ✅ 性能效率 (Performance)
- ✅ 质量率 (Quality)
- ✅ MTBF / MTTR 计算
- ✅ 吞吐量统计
- ✅ 性能快照和历史
- ✅ E10 时间模型

**使用示例：**

```csharp
var e116Tracker = new PerformanceTracker();

// 开始跟踪
e116Tracker.StartOperational();
e116Tracker.StartProductive();

// 记录处理
for (int i = 0; i < 100; i++)
{
    e116Tracker.RecordSubstrateProcessed(isSuccess: true);
}

// 记录停机
e116Tracker.RecordDowntime(300, isScheduled: false);

// 生成报告
string report = e116Tracker.GenerateReport();
Console.WriteLine(report);

// 输出:
// OEE: 85.50%
//   - Availability: 95.20%
//   - Performance: 92.80%
//   - Quality: 98.00%
// Throughput: 120.5 substrates/hour
// MTBF: 7200 sec, MTTR: 300 sec
```

## 🔄 集成使用

所有标准可以无缝集成使用：

```csharp
// 初始化所有管理器
var e39Service = new ObjectService("EqpObjects");
var e84Handler = new E84Handler("LP1", E84Mode.Active);
var e87Manager = new E87Manager("EQP-001");
var e90Tracker = new SubstrateTracker();
var e94Manager = new ControlJobManager();
var e116Tracker = new PerformanceTracker();

// 完整的处理流程
// 1. E39创建对象
var eqp = e39Service.CreateObject("EQP-001", "Equipment");

// 2. E87管理载体
e87Manager.AddLoadPort("LP1");
var carrier = e87Manager.RegisterCarrier("CARRIER-001");

// 3. E84载体交接
e84Handler.StartLoad();

// 4. E90跟踪基板
e90Tracker.RegisterSubstrate("WAFER-001");
e90Tracker.AssociateWithCarrier("WAFER-001", "CARRIER-001", 1);

// 5. E94创建控制作业
var cj = e94Manager.CreateControlJob("CJ-001", "RECIPE-001", 
    new List<string> { "CARRIER-001" },
    new Dictionary<string, List<string>> 
    { 
        { "CARRIER-001", new List<string> { "WAFER-001" } } 
    });

// 6. E116跟踪性能
e116Tracker.StartOperational();
e116Tracker.RecordSubstrateProcessed(true);
```

## 📊 性能指标示例

E116计算的关键指标：

| 指标 | 公式 | 目标值 |
|------|------|--------|
| **OEE** | Availability × Performance × Quality | > 85% |
| **Availability** | Operational Time / (Operational + Downtime) | > 90% |
| **Performance** | Ideal Cycle Time / Actual Cycle Time | > 95% |
| **Quality** | Good Count / Total Count | > 99% |
| **MTBF** | Operational Time / Failure Count | > 168 hrs |
| **MTTR** | Downtime / Repair Events | < 1 hr |

## 🎯 GEM300 支持

该实现支持GEM300标准集成，GEM300包含：

- ✅ **E30 (GEM)** - 通用设备模型（可扩展）
- ✅ **E37 (HSMS)** - 高速SECS消息（可扩展）
- ✅ **E84** - 载体交接 ✅
- ✅ **E87** - 载体管理 ✅
- ✅ **E90** - 基板跟踪 ✅
- ✅ **E94** - 控制作业 ✅
- ✅ **E116** - 性能跟踪 ✅

## 📝 使用场景

### 场景1：300mm Fab自动化

```csharp
// 设备接收载体，处理晶圆，跟踪性能
var system = new IntegratedFabSystem();
system.OnCarrierArrival("CARRIER-001");
system.ProcessAllWafers();
system.GeneratePerformanceReport();
```

### 场景2：MES集成

```csharp
// 与MES系统集成
e94Manager.ControlJobCreated += (s, e) => 
    MesInterface.SendControlJobCreated(e);

e90Tracker.SubstrateProcessed += (s, e) => 
    MesInterface.ReportWaferStatus(e);

e116Tracker.MetricsUpdated += (s, e) => 
    MesInterface.UpdateEquipmentPerformance(e);
```

### 场景3：AGV系统对接

```csharp
// E84与AGV对接
e84Handler.TransferCompleted += (s, e) =>
{
    AgvController.NotifyTransferComplete(e.LoadPortId);
};
```

## 🔧 扩展开发

### 添加自定义对象类型（E39）

```csharp
var customObj = objectService.CreateObject("TOOL-001", "CustomTool", 
    new Dictionary<string, object>
    {
        { "ToolType", "Etcher" },
        { "Chambers", 4 },
        { "MaxTemp", 400 }
    });
```

### 添加自定义性能指标（E116）

```csharp
public class CustomMetrics : PerformanceMetrics
{
    public double CustomKPI1 { get; set; }
    public double CustomKPI2 { get; set; }
    
    public override string ToString()
    {
        return base.ToString() + 
               $"\nCustom KPI1: {CustomKPI1}, KPI2: {CustomKPI2}";
    }
}
```

## 📚 参考文档

- [SEMI E39 Object Services](https://www.semi.org/en/semi-standards/e39)
- [SEMI E84 Carrier Handoff](https://www.semi.org/en/semi-standards/e84)
- [SEMI E87 Carrier Management](https://www.semi.org/en/semi-standards/e87)
- [SEMI E90 Substrate Tracking](https://www.semi.org/en/semi-standards/e90)
- [SEMI E94 Control Job Management](https://www.semi.org/en/semi-standards/e94)
- [SEMI E116 Equipment Performance Tracking](https://www.semi.org/en/semi-standards/e116)

## 📄 许可证

本项目仅供学习和参考使用。SEMI标准是SEMI（国际半导体设备和材料协会）的注册商标。

## 👥 贡献

欢迎提交问题和改进建议！

## ⚠️ 注意事项

1. **生产环境使用**：本实现用于教学和原型开发，生产环境需要添加：
   - 完整的错误处理和日志记录
   - 数据持久化
   - 安全机制
   - 硬件接口集成
   - 性能优化

2. **SEMI标准合规**：实现参考了SEMI标准规范，但未经过官方认证。

3. **.NET Framework版本**：本项目使用.NET Framework 4.7.2，确保目标系统已安装。

## 📞 联系方式

如有问题或建议，请提交Issue。

---

**版本：** 1.0.0  
**更新日期：** 2025-10-16  
**兼容性：** .NET Framework 4.7.2+  
