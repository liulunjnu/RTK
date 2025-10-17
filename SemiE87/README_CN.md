# SEMI E87 载体管理系统 (C# 实现)

## 概述

这是一个基于 **SEMI E87 (Carrier Management Specification)** 标准的 C# 实现。该系统提供了完整的载体管理功能，适用于半导体制造设备的自动化物料处理。

## SEMI E87 标准简介

SEMI E87 定义了半导体制造中载体管理的标准接口和行为规范，主要用于：
- **FOUP (Front Opening Unified Pod)** - 300mm晶圆载体
- **FOSB (Front Opening Shipping Box)** - 运输载体
- **SMIF (Standard Mechanical Interface)** - 标准机械接口

### 核心功能模块

1. **载体管理 (Carrier Management)**
   - 载体注册和跟踪
   - 载体ID读取和验证
   - 访问模式控制（自动/手动）

2. **装载端口管理 (Load Port Management)**
   - 端口状态监控
   - 载体装载/卸载操作
   - 传输状态管理

3. **位置管理 (Location Management)**
   - 载体位置跟踪
   - 载体-端口关联
   - 设备内位置管理

4. **槽位映射 (Slot Mapping)**
   - 晶圆位置检测
   - 槽位占用状态
   - 基板关联（E90集成）

## 项目结构

```
SemiE87/
├── E87Enums.cs           # 枚举定义（状态、模式等）
├── CarrierSlot.cs        # 载体槽位类
├── Carrier.cs            # 载体类
├── LoadPort.cs           # 装载端口类
├── E87Manager.cs         # E87管理器（主类）
├── Program.cs            # 示例程序
├── SemiE87.csproj        # 项目文件
└── README_CN.md          # 本文档
```

## 核心类说明

### 1. E87Manager - 主管理器

管理整个E87系统的核心类，协调载体和装载端口。

```csharp
var manager = new E87Manager("EQP-001");
manager.AddLoadPort("LP1", "Front-Left");
manager.RegisterCarrier("CARRIER-001", 25, "FOUP");
```

**主要方法：**
- `AddLoadPort()` - 添加装载端口
- `RegisterCarrier()` - 注册载体
- `LoadCarrierToPort()` - 装载载体
- `UnloadCarrierFromPort()` - 卸载载体
- `ReadCarrierId()` - 读取载体ID
- `PerformSlotMapping()` - 执行槽位映射

### 2. Carrier - 载体类

代表一个FOUP/FOSB载体。

```csharp
var carrier = new Carrier("CARRIER-001", 25);
carrier.AccessMode = CarrierAccessMode.Auto;
carrier.UpdateSlotMap(slotStatuses);
```

**主要属性：**
- `CarrierId` - 载体ID
- `Location` - 当前位置
- `AssociationState` - 关联状态
- `SlotMap` - 槽位映射表
- `AccessMode` - 访问模式

### 3. LoadPort - 装载端口类

代表设备上的一个载体装载位置。

```csharp
var loadPort = new LoadPort("LP1", "Front-Left");
loadPort.LoadCarrier(carrier);
loadPort.OpenDoor();
loadPort.CloseDoor();
loadPort.UnloadCarrier();
```

**主要属性：**
- `State` - 端口状态（Empty/Loading/Loaded/Unloading）
- `TransferState` - 传输状态
- `ClampState` - 夹紧状态
- `DoorState` - 门状态

## 使用示例

### 基本流程

```csharp
// 1. 创建管理器
var e87Manager = new E87Manager("EQP-12345");

// 2. 添加装载端口
e87Manager.AddLoadPort("LP1", "Front-Left");

// 3. 注册载体
var carrier = e87Manager.RegisterCarrier("CARRIER-001", 25, "FOUP");

// 4. 读取并验证载体ID
e87Manager.ReadCarrierId("LP1", "CARRIER-001");
e87Manager.VerifyCarrierId("CARRIER-001", "CARRIER-001");

// 5. 装载载体到端口
e87Manager.LoadCarrierToPort("CARRIER-001", "LP1");

// 6. 执行槽位映射
var slotMap = new Dictionary<int, SlotOccupancyStatus>
{
    { 1, SlotOccupancyStatus.Occupied },
    { 2, SlotOccupancyStatus.Occupied },
    { 3, SlotOccupancyStatus.Empty }
};
e87Manager.PerformSlotMapping("CARRIER-001", slotMap);

// 7. 开门访问晶圆
e87Manager.OpenLoadPortDoor("LP1");
// ... 处理晶圆 ...
e87Manager.CloseLoadPortDoor("LP1");

// 8. 卸载载体
e87Manager.UnloadCarrierFromPort("LP1");
```

### 事件订阅

```csharp
// 订阅载体事件
e87Manager.CarrierRegistered += (sender, e) =>
{
    Console.WriteLine($"Carrier {e.Carrier.CarrierId} registered");
};

// 订阅装载端口事件
e87Manager.LoadPortStateChanged += (sender, e) =>
{
    Console.WriteLine($"LoadPort {e.LoadPortId} state changed");
};

// 订阅载体到达事件
var loadPort = e87Manager.GetLoadPort("LP1");
loadPort.CarrierArrived += (sender, e) =>
{
    Console.WriteLine($"Carrier arrived at {e.LoadPortId}");
};
```

## 状态机说明

### 装载端口状态转换

```
Empty → Loading → Loaded → Unloading → Empty
         ↓                    ↑
       Error ←─────────────── Error
```

### 载体关联状态

```
NotAssociated → AssociatedWithLoadPort → AssociatedWithEquipment
      ↑                                           ↓
      └───────────────────────────────────────────┘
```

### 门状态转换

```
Closed → Opening → Open → Closing → Closed
   ↓                ↓        ↓         ↓
   └────────────→ Error ←────┘─────────┘
```

## 编译和运行

### 要求
- .NET 6.0 或更高版本

### 编译
```bash
cd SemiE87
dotnet build
```

### 运行示例程序
```bash
dotnet run
```

## 关键枚举类型

### CarrierAccessMode - 载体访问模式
- `Unknown` - 未知
- `Auto` - 自动模式
- `Manual` - 手动模式

### LoadPortState - 装载端口状态
- `Uninitialized` - 未初始化
- `Empty` - 空闲
- `Loading` - 装载中
- `Loaded` - 已装载
- `Unloading` - 卸载中
- `Error` - 错误

### CarrierLocation - 载体位置
- `Unknown` - 未知
- `AtLoadPort` - 在装载端口
- `InEquipment` - 在设备内
- `InTransit` - 传输中
- `NotAtEquipment` - 不在设备

### SlotOccupancyStatus - 槽位占用状态
- `Empty` - 空
- `Occupied` - 已占用
- `DoubleSlotted` - 双槽位
- `CrossSlotted` - 交叉占用
- `Unknown` - 未知

## 与GEM300集成

该实现支持 **GEM300** 标准的集成，GEM300包含以下SEMI标准：
- **E30 (GEM)** - 通用设备模型
- **E37 (HSMS)** - 高速SECS消息服务
- **E87 (CMS)** - 载体管理（本实现）
- **E90** - 基板跟踪
- **E94** - 控制作业管理
- **E157** - 设备性能跟踪

## 扩展功能

可以进一步扩展以下功能：

1. **SECS/GEM通信集成**
   ```csharp
   // 与MES系统通信
   SecsGemInterface secsGem = new SecsGemInterface();
   secsGem.SendCarrierArrivalEvent(carrier);
   ```

2. **数据库持久化**
   ```csharp
   // 保存载体历史记录
   CarrierHistoryRepository.Save(carrier);
   ```

3. **设备接口集成**
   ```csharp
   // 与硬件PLC通信
   PlcInterface plc = new PlcInterface();
   plc.ClampCarrier(loadPortId);
   ```

## 测试建议

1. **单元测试**
   - 测试载体状态转换
   - 测试装载端口操作
   - 测试异常情况处理

2. **集成测试**
   - 完整的载体处理流程
   - 多载体并发场景
   - 错误恢复机制

3. **性能测试**
   - 大量载体管理
   - 事件处理性能
   - 内存使用情况

## 许可证

本实现仅供学习和参考使用。SEMI E87是SEMI（国际半导体设备和材料协会）的注册标准。

## 联系方式

如有问题或建议，请联系开发团队。

---

**注意：** 这是一个教学实现，用于演示SEMI E87协议的核心概念。生产环境使用需要进一步完善错误处理、日志记录、安全机制等功能。
