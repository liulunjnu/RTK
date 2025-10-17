# GEM300 C# Implementation

这是一个完整的 GEM300 事件系统的 C# 实现，包含了 E39, E84, E87, E90, E94, E116 事件。

## 概述

本实现遵循 SEMI E30 (GEM300) 标准，提供了半导体设备通信的标准事件模型。

## 事件列表

### E39 - Processing State Change (加工状态变更)
设备加工状态发生变化时触发。

**状态包括：**
- INIT (初始化)
- IDLE (空闲)
- SETUP (设置)
- READY (就绪)
- EXECUTING (执行中)
- PAUSE (暂停)
- COMPLETE (完成)

### E84 - Material Received (材料接收)
设备接收到材料/基板时触发。

**包含信息：**
- 材料ID (Material ID)
- 端口ID (Port ID)
- 载具ID (Carrier ID)
- 槽位号 (Slot Number)
- 位置代码 (Location Code)

### E87 - Material Removed (材料移除)
材料/基板从设备移除时触发。

**包含信息：**
- 材料ID
- 端口ID
- 加工结果 (Processing Result: PASS/FAIL)

### E90 - Spooling Activity (数据假脱机活动)
数据假脱机活动（文件传输、数据记录等）时触发。

**活动类型：**
- START (开始)
- ACTIVE (活动中)
- COMPLETE (完成)
- FAILED (失败)
- ABORTED (中止)
- PAUSED (暂停)

### E94 - Control Job Status (控制作业状态)
控制作业状态变化时触发。

**状态类型：**
- QUEUED (排队)
- SELECTED (选中)
- WAITING (等待)
- EXECUTING (执行中)
- PAUSED (暂停)
- COMPLETED (完成)
- ABORTED (中止)
- STOPPED (停止)
- REJECTED (拒绝)

### E116 - Machine Data Collection (机器数据采集)
报告采集的设备/工艺数据时触发。

**可采集数据：**
- 温度、压力、功率等工艺参数
- 设备状态数据
- 报警数据

## 使用方法

### 1. 基本使用

```csharp
using GEM300;
using GEM300.Events;

// 获取事件管理器实例
var eventManager = Gem300EventManager.Instance;

// 订阅 E39 事件
eventManager.Subscribe("E39", (gemEvent) =>
{
    var e39 = gemEvent as E39ProcessingStateChange;
    Console.WriteLine($"状态从 {e39.PreviousState} 变更为 {e39.CurrentState}");
});

// 发布 E39 事件
var e39Event = new E39ProcessingStateChange(
    equipmentId: "EQP-001",
    previousState: E39ProcessingStateChange.ProcessingState.IDLE,
    currentState: E39ProcessingStateChange.ProcessingState.EXECUTING,
    reason: "开始执行配方"
);
eventManager.PublishEvent(e39Event);
```

### 2. 材料追踪 (E84/E87)

```csharp
// 材料接收
var e84Event = new E84MaterialReceived(
    equipmentId: "EQP-001",
    materialId: "LOT-12345",
    portId: "PORT-1",
    carrierId: "CARR-001",
    slotNumber: 1,
    locationCode: "LOAD_LOCK_1"
);
eventManager.PublishEvent(e84Event);

// 材料移除
var e87Event = new E87MaterialRemoved(
    equipmentId: "EQP-001",
    materialId: "LOT-12345",
    portId: "PORT-2",
    carrierId: "CARR-001",
    slotNumber: 1,
    locationCode: "UNLOAD_LOCK_1",
    processingResult: "PASS"
);
eventManager.PublishEvent(e87Event);
```

### 3. 控制作业管理 (E94)

```csharp
var e94Event = new E94ControlJobStatus(
    equipmentId: "EQP-001",
    controlJobId: "CJ-2023-1017-001",
    previousStatus: E94ControlJobStatus.ControlJobStatusType.WAITING,
    currentStatus: E94ControlJobStatus.ControlJobStatusType.EXECUTING,
    processProgramId: "RECIPE-ABC-123",
    materialIds: new List<string> { "LOT-12345", "LOT-12346" }
);
eventManager.PublishEvent(e94Event);
```

### 4. 数据采集 (E116)

```csharp
var e116Event = new E116MachineDataCollection(
    equipmentId: "EQP-001",
    reportId: "RPT-001",
    collectionEventId: "E116",
    dataCategory: "ProcessData",
    materialId: "LOT-12345"
);

// 添加采集的数据项
e116Event.AddDataItem("TEMP_CHAMBER", "腔室温度", 350.5, "°C");
e116Event.AddDataItem("PRESSURE_VAC", "真空压力", 1.2e-5, "Torr");
e116Event.AddDataItem("POWER_RF", "RF功率", 1500, "W");

eventManager.PublishEvent(e116Event);
```

### 5. 查询事件历史

```csharp
// 获取所有事件
var allEvents = eventManager.GetEventHistory();

// 获取特定类型的事件
var e39Events = eventManager.GetEventHistory("E39", count: 10);

// 按时间范围查询
var eventsInRange = eventManager.GetEventsByTimeRange(
    startTime: DateTime.UtcNow.AddHours(-1),
    endTime: DateTime.UtcNow
);
```

### 6. 全局事件处理

```csharp
// 订阅所有事件
eventManager.OnEventPublished += (gemEvent) =>
{
    Console.WriteLine($"事件发布: {gemEvent.EventId} - {gemEvent.EventName}");
};
```

## 项目结构

```
GEM300/
├── IGem300Event.cs              # 事件接口
├── Gem300EventBase.cs           # 事件基类
├── Gem300EventManager.cs        # 事件管理器
├── Events/
│   ├── E39ProcessingStateChange.cs
│   ├── E84MaterialReceived.cs
│   ├── E87MaterialRemoved.cs
│   ├── E90SpoolingActivity.cs
│   ├── E94ControlJobStatus.cs
│   └── E116MachineDataCollection.cs
├── Examples/
│   └── UsageExample.cs          # 使用示例
├── GEM300.csproj                # 项目文件
└── README.md                    # 本文档
```

## 编译和运行

```bash
# 编译项目
cd GEM300
dotnet build

# 运行示例
dotnet run --project Examples/UsageExample.cs
```

## 特性

- ✅ 完整实现 6 个 GEM300 标准事件 (E39, E84, E87, E90, E94, E116)
- ✅ 事件验证机制
- ✅ JSON 序列化支持
- ✅ 发布/订阅模式
- ✅ 事件历史记录
- ✅ 线程安全
- ✅ 单例模式的事件管理器
- ✅ 详细的中文注释和文档

## 依赖

- .NET 6.0 或更高版本
- System.Text.Json (用于序列化)

## 许可证

本实现遵循 Apache 2.0 许可证。

## 作者

GEM300 C# Implementation Team
