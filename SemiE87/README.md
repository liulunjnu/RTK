# SEMI E87 Carrier Management System (C# Implementation)

## Overview

This is a complete C# implementation of **SEMI E87 (Carrier Management Specification)** standard for semiconductor manufacturing equipment automation.

## What is SEMI E87?

SEMI E87 defines the standard interface and behavior for automated material handling systems in semiconductor fabrication facilities. It manages carriers (FOUPs, SMIFs, FOSBs) that hold wafers during manufacturing processes.

### Key Features

✅ **Carrier Management**
- Carrier registration and tracking
- Carrier ID reading and verification (RFID/Barcode)
- Access mode control (Auto/Manual)
- Carrier location tracking

✅ **Load Port Management**
- Port state monitoring (Empty/Loading/Loaded/Unloading)
- Carrier loading/unloading operations
- Transfer state management
- Door and clamp control

✅ **Slot Mapping**
- Wafer position detection (25 slots for 300mm FOUP)
- Slot occupancy status
- Substrate association (E90 integration ready)

✅ **Event-Driven Architecture**
- Real-time state change notifications
- Carrier arrival/departure events
- Complete event logging

## Project Structure

```
SemiE87/
├── E87Enums.cs           # Enumerations (states, modes, etc.)
├── CarrierSlot.cs        # Carrier slot information
├── Carrier.cs            # Carrier object (FOUP/FOSB)
├── LoadPort.cs           # Load port with state machine
├── E87Manager.cs         # Main E87 manager
├── Program.cs            # Demo application
├── SemiE87.csproj        # Project file
├── README.md             # This file
└── README_CN.md          # Chinese documentation
```

## Quick Start

### Basic Usage

```csharp
// 1. Create E87 Manager
var e87Manager = new E87Manager("EQP-12345");

// 2. Add Load Ports
e87Manager.AddLoadPort("LP1", "Front-Left");
e87Manager.AddLoadPort("LP2", "Front-Right");

// 3. Register Carrier
var carrier = e87Manager.RegisterCarrier("CARRIER-001", 25, "FOUP");

// 4. Read and Verify Carrier ID
e87Manager.ReadCarrierId("LP1", "CARRIER-001");
e87Manager.VerifyCarrierId("CARRIER-001", "CARRIER-001");

// 5. Load Carrier to Port
e87Manager.LoadCarrierToPort("CARRIER-001", "LP1");

// 6. Perform Slot Mapping
var slotMap = new Dictionary<int, SlotOccupancyStatus>
{
    { 1, SlotOccupancyStatus.Occupied },
    { 2, SlotOccupancyStatus.Occupied },
    { 3, SlotOccupancyStatus.Empty },
    { 4, SlotOccupancyStatus.Occupied }
};
e87Manager.PerformSlotMapping("CARRIER-001", slotMap);

// 7. Open Door and Access Wafers
e87Manager.OpenLoadPortDoor("LP1");
// ... process wafers ...
e87Manager.CloseLoadPortDoor("LP1");

// 8. Unload Carrier
e87Manager.UnloadCarrierFromPort("LP1");
```

### Event Subscription

```csharp
// Subscribe to carrier events
e87Manager.CarrierRegistered += (sender, e) =>
    Console.WriteLine($"Carrier {e.Carrier.CarrierId} registered");

e87Manager.CarrierIdRead += (sender, e) =>
    Console.WriteLine($"Carrier ID read: {e.Carrier.CarrierId}");

// Subscribe to load port events
e87Manager.LoadPortStateChanged += (sender, e) =>
    Console.WriteLine($"LoadPort {e.LoadPortId} state changed");

var loadPort = e87Manager.GetLoadPort("LP1");
loadPort.CarrierArrived += (sender, e) =>
    Console.WriteLine($"Carrier arrived at {e.LoadPortId}");
```

## Core Classes

### 1. E87Manager

Main controller for the entire E87 system.

**Key Methods:**
- `AddLoadPort(id, location)` - Add a load port
- `RegisterCarrier(id, slotCount, type)` - Register a carrier
- `LoadCarrierToPort(carrierId, loadPortId)` - Load carrier
- `UnloadCarrierFromPort(loadPortId)` - Unload carrier
- `ReadCarrierId(loadPortId, carrierId)` - Read carrier ID
- `PerformSlotMapping(carrierId, slotMap)` - Map wafer positions
- `GetSystemStatus()` - Get complete system status

### 2. Carrier

Represents a FOUP/FOSB carrier.

**Key Properties:**
- `CarrierId` - Unique carrier identifier
- `Location` - Current location (AtLoadPort, InEquipment, etc.)
- `AssociationState` - Association status
- `SlotMap` - List of 25 slots with occupancy status
- `AccessMode` - Auto or Manual access mode
- `IdStatus` - ID verification status

### 3. LoadPort

Represents a carrier loading position on equipment.

**Key Properties:**
- `State` - Port state (Empty/Loading/Loaded/Unloading)
- `TransferState` - Transfer status
- `ClampState` - Clamp status (Clamped/Unclamped)
- `DoorState` - Door status (Open/Closed/Opening/Closing)
- `AssociatedCarrier` - Currently loaded carrier

**Key Methods:**
- `LoadCarrier(carrier)` - Load a carrier
- `UnloadCarrier()` - Unload the carrier
- `OpenDoor()` - Open door for wafer access
- `CloseDoor()` - Close door

## State Machines

### Load Port State Transitions

```
Empty → Loading → Loaded → Unloading → Empty
         ↓                    ↑
       Error ←─────────────── Error
```

### Carrier Association States

```
NotAssociated → AssociatedWithLoadPort → AssociatedWithEquipment
      ↑                                           ↓
      └───────────────────────────────────────────┘
```

### Door State Transitions

```
Closed → Opening → Open → Closing → Closed
   ↓                ↓        ↓         ↓
   └────────────→ Error ←────┘─────────┘
```

## Build and Run

### Requirements
- .NET 6.0 or higher

### Build
```bash
cd SemiE87
dotnet build
```

### Run Demo
```bash
dotnet run
```

### Expected Output
```
=== SEMI E87 Carrier Management System Demo ===

Step 1: Adding Load Ports...
✓ Added 2 Load Ports

Step 2: Registering Carriers...
[EVENT] Carrier registered: CARRIER-001
[EVENT] Carrier registered: CARRIER-002
✓ Registered 2 Carriers

Step 3: Carrier Arrival Scenario...
  Reading Carrier ID at LP1...
[EVENT] Carrier ID read: CARRIER-001
  Verifying Carrier ID...
  Verification result: PASS ✓
  Loading Carrier to LP1...
✓ Carrier loaded successfully

... (more output)
```

## Key Enumerations

### CarrierAccessMode
- `Unknown`, `Auto`, `Manual`

### LoadPortState
- `Uninitialized`, `Empty`, `Loading`, `Loaded`, `Unloading`, `Error`

### CarrierLocation
- `Unknown`, `AtLoadPort`, `InEquipment`, `InTransit`, `NotAtEquipment`

### SlotOccupancyStatus
- `Empty`, `Occupied`, `DoubleSlotted`, `CrossSlotted`, `Unknown`

### CarrierIdStatus
- `NotRead`, `WaitingForRead`, `IdRead`, `IdVerified`, `IdVerificationFailed`

## GEM300 Integration

This implementation is designed to integrate with **GEM300** standard, which includes:
- **E30 (GEM)** - Generic Equipment Model
- **E37 (HSMS)** - High-Speed SECS Message Services
- **E87 (CMS)** - Carrier Management (this implementation)
- **E90** - Substrate Tracking
- **E94** - Control Job Management
- **E157** - Equipment Performance Tracking

## Extension Points

The implementation can be extended with:

1. **SECS/GEM Communication**
   ```csharp
   SecsGemInterface secsGem = new SecsGemInterface();
   secsGem.SendCarrierArrivalEvent(carrier);
   ```

2. **Database Persistence**
   ```csharp
   CarrierHistoryRepository.Save(carrier);
   ```

3. **Hardware Integration**
   ```csharp
   PlcInterface plc = new PlcInterface();
   plc.ClampCarrier(loadPortId);
   plc.OpenDoor(loadPortId);
   ```

4. **MES Integration**
   ```csharp
   MesInterface mes = new MesInterface();
   mes.ReportCarrierArrival(carrier);
   ```

## Testing Recommendations

1. **Unit Tests**
   - Carrier state transitions
   - Load port operations
   - Exception handling

2. **Integration Tests**
   - Complete carrier processing flow
   - Multi-carrier concurrent scenarios
   - Error recovery mechanisms

3. **Performance Tests**
   - Large-scale carrier management
   - Event processing performance
   - Memory usage analysis

## License

This implementation is for educational and reference purposes. SEMI E87 is a registered standard of SEMI (Semiconductor Equipment and Materials International).

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      E87Manager                              │
│  - Coordinates carriers and load ports                      │
│  - Manages system state                                     │
│  - Publishes events                                         │
└──────────┬──────────────────────────────────┬───────────────┘
           │                                  │
           ↓                                  ↓
┌──────────────────────┐          ┌──────────────────────┐
│     LoadPort         │          │      Carrier         │
│  - State machine     │◄────────►│  - SlotMap           │
│  - Door control      │          │  - Location tracking │
│  - Clamp control     │          │  - ID verification   │
└──────────────────────┘          └──────────────────────┘
           │                                  │
           │         ┌──────────────────────┐ │
           └────────►│   CarrierSlot        │◄┘
                     │  - Wafer position    │
                     │  - Occupancy status  │
                     └──────────────────────┘
```

## Contributing

Contributions are welcome! Please ensure code follows SEMI E87 specification guidelines.

---

**Note:** This is an educational implementation demonstrating core SEMI E87 concepts. Production use requires additional error handling, logging, security, and hardware integration.
