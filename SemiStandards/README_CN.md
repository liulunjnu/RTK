# SEMI 标准完整实现 (.NET Framework 4.7.2)

[English](README.md) | 简体中文

## 🎯 项目简介

这是一个基于.NET Framework 4.7.2的完整SEMI（国际半导体设备和材料协会）标准C#实现库，包含6个核心标准：

- **E39** - 对象服务标准
- **E84** - 增强型载体交接
- **E87** - 载体管理规范
- **E90** - 基板跟踪标准
- **E94** - 控制作业管理
- **E116** - 设备性能跟踪

适用于半导体制造设备的自动化控制、MES集成、AGV系统对接等场景。

## 🌟 核心特性

### ✅ 完整的标准实现
- 6个SEMI标准的完整C#实现
- 符合SEMI规范的接口设计
- 事件驱动架构

### ✅ 生产级代码质量
- 清晰的代码结构和注释
- 完整的错误处理
- 可扩展的设计模式

### ✅ 无缝集成
- 所有标准可独立使用
- 标准间可无缝集成
- 统一的事件和接口

### ✅ 实战演示
- 完整的Demo程序
- 详细的使用示例
- 集成场景演示

## 📦 标准详解

### E39 - 对象服务

**用途：** 为设备内的所有实体提供统一的对象模型

**核心类：**
- `SemiObject` - SEMI对象基类
- `ObjectRepository` - 对象仓库
- `ObjectService` - 对象服务管理器

**主要功能：**
```csharp
// 创建设备对象
var equipment = objectService.CreateObject("EQP-001", "Equipment");

// 添加自定义属性
equipment.AddAttribute("Model", "300mm Processor", AttributeAccess.ReadWrite);

// 修改属性
objectService.ModifyObjectAttribute("EQP-001", "Model", "300mm Processor v2");

// 查询对象
var equipments = objectService.GetObjectsByType("Equipment");
```

**应用场景：**
- 设备配置管理
- 动态对象创建
- 统一的数据访问接口

---

### E84 - 载体交接

**用途：** 实现设备之间的载体自动交接握手协议

**核心类：**
- `E84StateMachine` - E84状态机
- `E84Signals` - 并行I/O信号
- `E84Handler` - E84处理器

**信号定义：**
```
主动端输出: L_REQ, U_REQ, READY, BUSY, COMPT, CONT
主动端输入: VALID, CS_0, CS_1, TR_REQ, HO_AVBL, ES
```

**主要功能：**
```csharp
// 创建E84处理器（主动模式）
var e84 = new E84Handler("LP1", E84Mode.Active);

// 开始装载传输
e84.StartLoad();

// 更新输入信号
var signals = new E84Signals 
{ 
    VALID = true,    // 载体在位
    TR_REQ = true    // 传输请求
};
e84.UpdateInputs(signals);
```

**状态转换：**
```
ReadyToTransfer → TransferRequested → TransferReady 
→ Transferring → TransferComplete → ReadyToTransfer
```

**应用场景：**
- AGV与设备间的载体交接
- 设备与设备间的载体传递
- Stocker与设备的载体交互

---

### E87 - 载体管理

**用途：** 管理FOUP/SMIF载体及装载端口

**核心类：**
- `Carrier` - 载体对象
- `LoadPort` - 装载端口
- `E87Manager` - E87管理器

**主要功能：**
```csharp
var e87 = new E87Manager("EQP-001");

// 添加装载端口
e87.AddLoadPort("LP1");
e87.AddLoadPort("LP2");

// 注册载体
var carrier = e87.RegisterCarrier("CARRIER-001", 25);  // 25槽位FOUP

// 装载载体到端口
e87.LoadCarrierToPort("CARRIER-001", "LP1");

// 槽位映射
var slotMap = new Dictionary<int, SlotOccupancyStatus>
{
    { 1, SlotOccupancyStatus.Occupied },  // 槽位1有晶圆
    { 2, SlotOccupancyStatus.Occupied },  // 槽位2有晶圆
    { 3, SlotOccupancyStatus.Empty }      // 槽位3空
};
carrier.UpdateSlotMap(slotMap);

// 卸载载体
e87.UnloadCarrierFromPort("LP1");
```

**载体状态：**
- NotAtEquipment - 不在设备
- AtLoadPort - 在装载端口
- InEquipment - 在设备内
- InTransit - 传输中

**应用场景：**
- FOUP载体管理
- 装载端口监控
- 载体ID读取和验证
- 槽位映射

---

### E90 - 基板跟踪

**用途：** 跟踪单个晶圆（基板）的位置和处理状态

**核心类：**
- `Substrate` - 基板对象
- `SubstrateTracker` - 基板跟踪器

**主要功能：**
```csharp
var e90 = new SubstrateTracker();

// 注册基板
var wafer = e90.RegisterSubstrate("WAFER-001", SubstrateType.ProductWafer);

// 关联到载体
e90.AssociateWithCarrier("WAFER-001", "CARRIER-001", slotNumber: 1);

// 更新位置
e90.UpdateLocation("WAFER-001", SubstrateState.AtStation, "ProcessChamber-1");

// 开始处理
e90.StartProcessing("WAFER-001", "Chamber-1", "RECIPE-001");

// 完成处理
e90.CompleteProcessing("WAFER-001", SubstrateProcessResult.Success);

// 获取历史
var history = wafer.GetHistory();
foreach (var entry in history)
{
    Console.WriteLine(entry);  // [时间] 事件 @ 位置
}

// 统计数据
var stats = e90.GetStatistics();
Console.WriteLine($"成功: {stats["Success"]}, 失败: {stats["Failed"]}");
```

**基板状态：**
- InCarrier - 在载体中
- AtLoadPort - 在装载端口
- InEquipment - 在设备内
- AtStation - 在工位
- Processing - 处理中
- Completed - 已完成

**应用场景：**
- 晶圆级追溯
- WIP (在制品) 管理
- 产量统计
- 异常晶圆跟踪

---

### E94 - 控制作业管理

**用途：** 管理批次处理作业

**核心类：**
- `ControlJob` - 控制作业
- `CarrierJob` - 载体作业
- `ProcessJob` - 处理作业
- `ControlJobManager` - 作业管理器

**主要功能：**
```csharp
var e94 = new ControlJobManager();

// 创建控制作业
var carrierIds = new List<string> { "CARRIER-001", "CARRIER-002" };
var substratesByCarrier = new Dictionary<string, List<string>>
{
    { "CARRIER-001", new List<string> { "W1", "W2", "W3" } },
    { "CARRIER-002", new List<string> { "W4", "W5", "W6" } }
};

var controlJob = e94.CreateControlJob(
    controlJobId: "CJ-2025-001",
    processRecipe: "RECIPE-CVD-001",
    carrierIds: carrierIds,
    substratesByCarrier: substratesByCarrier
);

// 开始作业
e94.StartControlJob("CJ-2025-001");

// 处理单个基板
e94.StartProcessJob("CJ-2025-001", "W1", "Chamber-1");
// ... 处理 ...
e94.CompleteProcessJob("CJ-2025-001", "W1", "SUCCESS");

// 暂停/恢复
e94.PauseControlJob("CJ-2025-001");
e94.ResumeControlJob("CJ-2025-001");

// 查看进度
Console.WriteLine(controlJob);  
// ControlJob[CJ-2025-001] State=Executing, Progress=3/6
```

**作业状态：**
- Queued - 排队中
- Executing - 执行中
- Paused - 已暂停
- Completed - 已完成
- Canceled - 已取消

**应用场景：**
- 批次作业调度
- 生产计划执行
- 多载体协同处理
- 作业进度监控

---

### E116 - 性能跟踪

**用途：** 跟踪设备性能指标（OEE、MTBF、MTTR等）

**核心类：**
- `PerformanceMetrics` - 性能指标
- `PerformanceTracker` - 性能跟踪器

**主要功能：**
```csharp
var e116 = new PerformanceTracker();

// 开始运行跟踪
e116.StartOperational();
e116.StartProductive();

// 记录处理
for (int i = 0; i < 100; i++)
{
    // 处理晶圆...
    bool success = ProcessWafer(i);
    e116.RecordSubstrateProcessed(success);
}

// 记录停机
e116.StopProductive();
e116.RecordDowntime(seconds: 300, isScheduled: false);  // 非计划停机5分钟

// 继续运行
e116.StartProductive();

// 生成报告
string report = e116.GenerateReport();
Console.WriteLine(report);
```

**输出示例：**
```
=== Performance Report ===
Report Time: 2025-10-16 14:30:00

Performance Metrics:
  OEE: 85.50%
    - Availability: 95.20%
    - Performance: 92.80%
    - Quality: 98.00%
  Throughput: 120.5 substrates/hour
  MTBF: 7200 sec, MTTR: 300 sec
  Processed: 100 (Success: 98, Failed: 2)

Time Breakdown:
  Operational: 3600 sec
  Productive: 3300 sec
  Scheduled Downtime: 0 sec
  Unscheduled Downtime: 300 sec
```

**关键指标：**
| 指标 | 说明 | 计算公式 |
|------|------|----------|
| **OEE** | 综合设备效率 | Availability × Performance × Quality |
| **Availability** | 可用度 | 运行时间 / (运行时间 + 停机时间) |
| **Performance** | 性能效率 | 理论周期时间 / 实际周期时间 |
| **Quality** | 质量率 | 合格品数 / 总产品数 |
| **MTBF** | 平均故障间隔 | 运行时间 / 故障次数 |
| **MTTR** | 平均修复时间 | 停机时间 / 修复次数 |

**应用场景：**
- 设备效率分析
- 预防性维护
- 产能规划
- KPI报告

---

## 🔄 集成使用示例

所有标准可以无缝协作：

```csharp
// 初始化所有管理器
var e39 = new ObjectService("Equipment");
var e84 = new E84Handler("LP1", E84Mode.Active);
var e87 = new E87Manager("EQP-001");
var e90 = new SubstrateTracker();
var e94 = new ControlJobManager();
var e116 = new PerformanceTracker();

// ========== 完整的生产流程 ==========

// 1. E39: 创建设备对象
var equipment = e39.CreateObject("EQP-001", "Equipment");

// 2. E87: 载体管理
e87.AddLoadPort("LP1");
var carrier = e87.RegisterCarrier("CARRIER-001", 25);

// 3. E84: 载体交接（与AGV）
e84.StartLoad();
// ... E84握手过程 ...

// 4. E87: 装载载体
e87.LoadCarrierToPort("CARRIER-001", "LP1");

// 5. E90: 注册基板
var waferIds = new List<string> { "W1", "W2", "W3" };
foreach (var wId in waferIds)
{
    e90.RegisterSubstrate(wId);
    e90.AssociateWithCarrier(wId, "CARRIER-001", waferIds.IndexOf(wId) + 1);
}

// 6. E94: 创建控制作业
var controlJob = e94.CreateControlJob("CJ-001", "RECIPE-001",
    new List<string> { "CARRIER-001" },
    new Dictionary<string, List<string>> { { "CARRIER-001", waferIds } });

e94.StartControlJob("CJ-001");

// 7. E116: 开始性能跟踪
e116.StartOperational();
e116.StartProductive();

// 8. 处理所有晶圆
foreach (var wId in waferIds)
{
    // E90: 更新基板位置
    e90.UpdateLocation(wId, SubstrateState.AtStation, "Chamber-1");
    
    // E90: 开始处理
    e90.StartProcessing(wId, "Chamber-1", "RECIPE-001");
    
    // E94: 开始处理作业
    e94.StartProcessJob("CJ-001", wId, "Chamber-1");
    
    // ... 实际处理过程 ...
    System.Threading.Thread.Sleep(1000);
    
    // E90: 完成处理
    e90.CompleteProcessing(wId, SubstrateProcessResult.Success);
    
    // E94: 完成处理作业
    e94.CompleteProcessJob("CJ-001", wId, "SUCCESS");
    
    // E116: 记录
    e116.RecordSubstrateProcessed(true);
}

// 9. E116: 生成性能报告
e116.StopProductive();
e116.StopOperational();
Console.WriteLine(e116.GenerateReport());

// 10. E87: 卸载载体
e87.UnloadCarrierFromPort("LP1");

// 11. E84: 载体交接（返回AGV）
e84.StartUnload();
```

## 🏭 实际应用场景

### 场景1：300mm晶圆厂自动化

```
AGV运送FOUP → E84握手 → E87装载 → E90跟踪晶圆 
→ E94执行批次作业 → E116记录性能 → E87卸载 → E84交接给AGV
```

### 场景2：MES系统集成

```csharp
// 订阅事件并上报MES
e87Manager.EventOccurred += (s, e) => MES.ReportCarrierEvent(e);
e90Tracker.SubstrateStateChanged += (s, e) => MES.UpdateWaferStatus(e);
e94Manager.ControlJobCompleted += (s, e) => MES.ReportBatchComplete(e);
e116Tracker.MetricsUpdated += (s, e) => MES.UpdateEquipmentOEE(e);
```

### 场景3：多工站协同

```csharp
// 工站1: 光刻
e90.UpdateLocation(waferId, SubstrateState.AtStation, "Litho-1");
e90.StartProcessing(waferId, "Litho-1", "LITHO_RECIPE");

// 工站2: 蚀刻
e90.UpdateLocation(waferId, SubstrateState.AtStation, "Etch-1");
e90.StartProcessing(waferId, "Etch-1", "ETCH_RECIPE");

// 工站3: 清洗
e90.UpdateLocation(waferId, SubstrateState.AtStation, "Clean-1");
e90.StartProcessing(waferId, "Clean-1", "CLEAN_RECIPE");
```

## 📊 性能指标参考值

| 指标 | 世界级 | 优秀 | 良好 | 需改进 |
|------|--------|------|------|--------|
| **OEE** | > 85% | 75-85% | 60-75% | < 60% |
| **Availability** | > 95% | 90-95% | 85-90% | < 85% |
| **Performance** | > 95% | 90-95% | 85-90% | < 85% |
| **Quality** | > 99% | 98-99% | 95-98% | < 95% |
| **MTBF** | > 168h | 100-168h | 48-100h | < 48h |
| **MTTR** | < 30min | 30-60min | 1-2h | > 2h |

## 🚀 快速开始

### 1. 克隆或下载项目

```bash
git clone <repository-url>
cd SemiStandards
```

### 2. 打开解决方案

使用Visual Studio 2017或更高版本打开 `SemiStandards.sln`

### 3. 编译项目

```
Ctrl + Shift + B (Visual Studio)
或
msbuild SemiStandards.sln /p:Configuration=Release
```

### 4. 运行Demo

```bash
cd SemiStandards.Demo\bin\Debug
SemiStandards.Demo.exe
```

## 📖 API文档

详细的API文档请参考各项目中的XML注释。

### 主要命名空间

- `SemiStandards.Common` - 公共类型和接口
- `SemiE39` - 对象服务
- `SemiE84` - 载体交接
- `SemiE87` - 载体管理
- `SemiE90` - 基板跟踪
- `SemiE94` - 控制作业
- `SemiE116` - 性能跟踪

## 🔧 高级用法

### 自定义对象类型

```csharp
// 创建自定义工具对象
var customTool = objectService.CreateObject("TOOL-001", "CustomTool", 
    new Dictionary<string, object>
    {
        { "ToolType", "Etcher" },
        { "Chambers", 4 },
        { "ProcessGas", "CF4" },
        { "MaxPressure", 100.0 }
    });
```

### 自定义性能指标

```csharp
public class ExtendedMetrics : PerformanceMetrics
{
    public double ChambersUtilization { get; set; }
    public double GasConsumption { get; set; }
    public double PowerConsumption { get; set; }
}
```

### 事件订阅模式

```csharp
// 统一的事件处理
void SubscribeAllEvents()
{
    e87.EventOccurred += OnE87Event;
    e90.SubstrateRegistered += OnE90Event;
    e94.ControlJobStateChanged += OnE94Event;
    e116.MetricsUpdated += OnE116Event;
}

void OnE87Event(object sender, SemiEventArgs e)
{
    LogEvent($"E87: {e.EventName} at {e.EventTime}");
}
```

## ⚠️ 注意事项

1. **本项目为教学和原型开发使用**
   - 生产环境需要添加完整的错误处理
   - 需要实现数据持久化
   - 需要集成实际硬件接口

2. **SEMI标准合规**
   - 实现参考SEMI官方规范
   - 未经过SEMI官方认证
   - 建议用于学习和原型验证

3. **.NET Framework要求**
   - 需要.NET Framework 4.7.2或更高版本
   - 适用于Windows平台

## 📞 技术支持

如有问题或建议，请：
1. 提交GitHub Issue
2. 查看示例代码
3. 阅读SEMI官方文档

## 📄 许可证

本项目仅供学习和参考使用。SEMI标准是国际半导体设备和材料协会的注册商标。

---

**版本：** 1.0.0  
**更新日期：** 2025-10-16  
**目标框架：** .NET Framework 4.7.2  
**开发环境：** Visual Studio 2017+
