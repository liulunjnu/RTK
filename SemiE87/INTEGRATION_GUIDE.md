# SEMI E87 Integration Guide

## Overview

This guide explains how to integrate the E87 implementation with other semiconductor manufacturing systems and protocols.

## 1. GEM300 Integration

GEM300 is a comprehensive standard that combines multiple SEMI standards. Here's how E87 fits into GEM300:

### GEM300 Components

```
┌──────────────────────────────────────────────────────────┐
│                      GEM300 Suite                         │
├──────────────────────────────────────────────────────────┤
│  E30 (GEM)      - Generic Equipment Model                │
│  E37 (HSMS)     - High-Speed SECS Message Services       │
│  E87 (CMS)      - Carrier Management (This Implementation)│
│  E90 (STM)      - Substrate Tracking                     │
│  E94 (CJM)      - Control Job Management                 │
│  E157 (EPT)     - Equipment Performance Tracking         │
└──────────────────────────────────────────────────────────┘
```

### Integration Example

```csharp
public class Gem300System
{
    private E87Manager e87Manager;
    private SecsGemDriver secsGem;
    private SubstrateTracker e90Tracker;
    
    public Gem300System(string equipmentId)
    {
        // Initialize E87 (Carrier Management)
        e87Manager = new E87Manager(equipmentId);
        
        // Initialize SECS/GEM (E30/E37)
        secsGem = new SecsGemDriver(equipmentId);
        
        // Initialize E90 (Substrate Tracking)
        e90Tracker = new SubstrateTracker();
        
        // Wire up events
        WireEvents();
    }
    
    private void WireEvents()
    {
        // E87 -> SECS/GEM Events
        e87Manager.CarrierArrived += (sender, e) =>
        {
            // Send S6F11 - Event Report: Carrier Arrived
            secsGem.SendEventReport(1001, new
            {
                CarrierId = e.Carrier.CarrierId,
                LoadPortId = e.LoadPortId,
                Time = DateTime.Now
            });
        };
        
        e87Manager.CarrierIdRead += (sender, e) =>
        {
            // Send S6F11 - Event Report: Carrier ID Read
            secsGem.SendEventReport(1002, new
            {
                CarrierId = e.Carrier.CarrierId,
                IdStatus = e.Carrier.IdStatus
            });
        };
        
        // E87 -> E90 Integration
        e87Manager.SlotMapVerified += (sender, e) =>
        {
            // Update substrate tracking
            foreach (var slot in e.Carrier.SlotMap)
            {
                if (slot.OccupancyStatus == SlotOccupancyStatus.Occupied)
                {
                    e90Tracker.RegisterSubstrate(
                        slot.SubstrateId,
                        e.Carrier.CarrierId,
                        slot.SlotNumber
                    );
                }
            }
        };
    }
}
```

## 2. SECS/GEM (E30/E37) Integration

### Common SECS-II Messages for E87

#### Stream 1: Equipment Status

```csharp
// S1F3 - Request Equipment Status
public void HandleS1F3Request()
{
    var status = new
    {
        OnlineStatus = e87Manager.IsOnline,
        LoadPorts = e87Manager.GetAllLoadPorts().Select(lp => new
        {
            LoadPortId = lp.LoadPortId,
            State = lp.State.ToString(),
            CarrierId = lp.AssociatedCarrier?.CarrierId ?? ""
        })
    };
    
    secsGem.SendS1F4Reply(status);
}
```

#### Stream 6: Data Collection

```csharp
// S6F11 - Event Report
public class E87EventReporter
{
    private const int EVENT_CARRIER_ARRIVED = 1001;
    private const int EVENT_CARRIER_DEPARTED = 1002;
    private const int EVENT_CARRIER_ID_READ = 1003;
    private const int EVENT_SLOT_MAP_VERIFIED = 1004;
    private const int EVENT_LOAD_PORT_STATE_CHANGED = 1005;
    
    public void ReportCarrierArrived(Carrier carrier, string loadPortId)
    {
        var eventData = new Dictionary<string, object>
        {
            { "CARRIER_ID", carrier.CarrierId },
            { "LOAD_PORT_ID", loadPortId },
            { "TIME", DateTime.Now },
            { "CARRIER_TYPE", carrier.CarrierType },
            { "SLOT_COUNT", carrier.SlotCount }
        };
        
        secsGem.SendEventReport(EVENT_CARRIER_ARRIVED, eventData);
    }
}
```

#### Stream 7: Process Program Management

```csharp
// S7F3 - Process Program Request
public void HandleProcessProgramRequest(string carrierId)
{
    var carrier = e87Manager.GetCarrier(carrierId);
    
    if (carrier != null)
    {
        // Get process program based on carrier
        var ppid = GetProcessProgramId(carrier);
        secsGem.SendS7F4Reply(ppid);
    }
}
```

## 3. E90 (Substrate Tracking) Integration

### Substrate Association

```csharp
public class E90Integration
{
    private E87Manager e87Manager;
    private SubstrateTrackingManager e90Manager;
    
    public void AssociateSubstratesToCarrier(string carrierId, List<string> substrateIds)
    {
        var carrier = e87Manager.GetCarrier(carrierId);
        
        for (int i = 0; i < substrateIds.Count && i < carrier.SlotCount; i++)
        {
            var slot = carrier.SlotMap[i];
            slot.SubstrateId = substrateIds[i];
            slot.OccupancyStatus = SlotOccupancyStatus.Occupied;
            slot.SubstrateAssociation = SubstrateAssociationState.Associated;
            
            // Register in E90
            e90Manager.RegisterSubstrate(new Substrate
            {
                SubstrateId = substrateIds[i],
                CarrierId = carrierId,
                SlotNumber = i + 1,
                State = SubstrateState.AtLoadPort
            });
        }
        
        carrier.SlotMapStatus = SlotMapStatus.Verified;
    }
    
    public void TrackSubstrateMovement(string substrateId, string fromLocation, string toLocation)
    {
        // Update E90 tracking
        e90Manager.UpdateSubstrateLocation(substrateId, toLocation);
        
        // Update carrier slot if applicable
        var carrier = FindCarrierBySubstrate(substrateId);
        if (carrier != null)
        {
            carrier.Location = ParseLocation(toLocation);
        }
    }
}
```

## 4. E94 (Control Job Management) Integration

### Control Job with Carrier Management

```csharp
public class ControlJobManager
{
    private E87Manager e87Manager;
    
    public class ControlJob
    {
        public string JobId { get; set; }
        public List<string> CarrierIds { get; set; }
        public string ProcessProgram { get; set; }
        public ControlJobState State { get; set; }
    }
    
    public bool CreateControlJob(ControlJob job)
    {
        // Verify all carriers are available
        foreach (var carrierId in job.CarrierIds)
        {
            var carrier = e87Manager.GetCarrier(carrierId);
            if (carrier == null || carrier.IsInUse)
            {
                return false;
            }
        }
        
        // Mark carriers as in use
        foreach (var carrierId in job.CarrierIds)
        {
            var carrier = e87Manager.GetCarrier(carrierId);
            carrier.IsInUse = true;
        }
        
        // Create control job
        job.State = ControlJobState.Queued;
        SaveControlJob(job);
        
        return true;
    }
    
    public void StartControlJob(string jobId)
    {
        var job = GetControlJob(jobId);
        
        // Load carriers to available ports
        var availablePorts = e87Manager.GetAvailableLoadPorts();
        
        for (int i = 0; i < job.CarrierIds.Count && i < availablePorts.Count; i++)
        {
            e87Manager.LoadCarrierToPort(
                job.CarrierIds[i],
                availablePorts[i].LoadPortId
            );
        }
        
        job.State = ControlJobState.Processing;
    }
}
```

## 5. MES (Manufacturing Execution System) Integration

### MES Interface

```csharp
public class MesInterface
{
    private E87Manager e87Manager;
    private HttpClient httpClient;
    
    public async Task ReportCarrierArrival(Carrier carrier, string loadPortId)
    {
        var data = new
        {
            EquipmentId = e87Manager.EquipmentId,
            CarrierId = carrier.CarrierId,
            LoadPortId = loadPortId,
            Timestamp = DateTime.Now,
            CarrierType = carrier.CarrierType,
            SlotCount = carrier.SlotCount,
            OccupiedSlots = carrier.GetOccupiedSlotCount()
        };
        
        await httpClient.PostAsJsonAsync("/api/carrier-arrival", data);
    }
    
    public async Task<string> RequestProcessRecipe(string carrierId)
    {
        var response = await httpClient.GetAsync($"/api/recipe?carrierId={carrierId}");
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task ReportProcessComplete(string carrierId, ProcessResult result)
    {
        var data = new
        {
            CarrierId = carrierId,
            Result = result,
            ProcessedWafers = result.WaferCount,
            Timestamp = DateTime.Now
        };
        
        await httpClient.PostAsJsonAsync("/api/process-complete", data);
    }
}
```

## 6. Hardware Integration (PLC/Robot)

### PLC Interface for Physical Control

```csharp
public class PlcInterface
{
    private TcpClient plcClient;
    
    // Read carrier presence sensor
    public bool IsCarrierPresent(string loadPortId)
    {
        var address = GetSensorAddress(loadPortId, "PRESENCE");
        return ReadPLCBit(address);
    }
    
    // Control clamp actuator
    public void ClampCarrier(string loadPortId)
    {
        var address = GetActuatorAddress(loadPortId, "CLAMP");
        WritePLCBit(address, true);
        
        // Wait for clamped confirmation
        var confirmAddress = GetSensorAddress(loadPortId, "CLAMPED");
        WaitForPLCBit(confirmAddress, true, timeout: 5000);
    }
    
    // Control door
    public void OpenDoor(string loadPortId)
    {
        var address = GetActuatorAddress(loadPortId, "DOOR_OPEN");
        WritePLCBit(address, true);
        
        // Wait for door open confirmation
        var confirmAddress = GetSensorAddress(loadPortId, "DOOR_OPENED");
        WaitForPLCBit(confirmAddress, true, timeout: 10000);
    }
    
    // Read slot map from optical sensor
    public Dictionary<int, SlotOccupancyStatus> ReadSlotMapFromSensor(string loadPortId)
    {
        var slotMap = new Dictionary<int, SlotOccupancyStatus>();
        
        for (int slot = 1; slot <= 25; slot++)
        {
            var address = GetSensorAddress(loadPortId, $"SLOT_{slot}");
            bool isOccupied = ReadPLCBit(address);
            
            slotMap[slot] = isOccupied 
                ? SlotOccupancyStatus.Occupied 
                : SlotOccupancyStatus.Empty;
        }
        
        return slotMap;
    }
}
```

### RFID Reader Integration

```csharp
public class RfidReader
{
    private SerialPort rfidPort;
    
    public string ReadCarrierId(string loadPortId)
    {
        // Send read command to RFID reader
        rfidPort.WriteLine($"READ:{loadPortId}");
        
        // Wait for response
        var response = rfidPort.ReadLine();
        
        // Parse carrier ID from response
        var carrierId = ParseRfidResponse(response);
        
        return carrierId;
    }
    
    public bool WriteCarrierId(string loadPortId, string carrierId)
    {
        rfidPort.WriteLine($"WRITE:{loadPortId}:{carrierId}");
        var response = rfidPort.ReadLine();
        
        return response.Contains("OK");
    }
}
```

## 7. Database Persistence

### Carrier History Tracking

```csharp
public class CarrierHistoryRepository
{
    private DbContext dbContext;
    
    public void LogCarrierEvent(Carrier carrier, string eventType, string details)
    {
        var historyEntry = new CarrierHistory
        {
            CarrierId = carrier.CarrierId,
            EventType = eventType,
            EventTime = DateTime.Now,
            Location = carrier.Location.ToString(),
            AssociatedLoadPort = carrier.AssociatedLoadPortId,
            Details = details
        };
        
        dbContext.CarrierHistory.Add(historyEntry);
        dbContext.SaveChanges();
    }
    
    public List<CarrierHistory> GetCarrierHistory(string carrierId, DateTime startTime)
    {
        return dbContext.CarrierHistory
            .Where(h => h.CarrierId == carrierId && h.EventTime >= startTime)
            .OrderBy(h => h.EventTime)
            .ToList();
    }
}
```

## 8. Complete Integration Example

### Full System Integration

```csharp
public class IntegratedE87System
{
    private E87Manager e87Manager;
    private SecsGemDriver secsGem;
    private MesInterface mes;
    private PlcInterface plc;
    private RfidReader rfid;
    private CarrierHistoryRepository history;
    
    public IntegratedE87System(string equipmentId)
    {
        e87Manager = new E87Manager(equipmentId);
        secsGem = new SecsGemDriver(equipmentId);
        mes = new MesInterface();
        plc = new PlcInterface();
        rfid = new RfidReader();
        history = new CarrierHistoryRepository();
        
        WireIntegratedEvents();
    }
    
    private void WireIntegratedEvents()
    {
        e87Manager.LoadPortStateChanged += async (sender, e) =>
        {
            var loadPort = e87Manager.GetLoadPort(e.LoadPortId);
            
            if (loadPort.State == LoadPortState.Loaded)
            {
                // 1. Read carrier ID from RFID
                var carrierId = rfid.ReadCarrierId(e.LoadPortId);
                
                // 2. Update E87 system
                e87Manager.ReadCarrierId(e.LoadPortId, carrierId);
                
                // 3. Read slot map from PLC sensors
                var slotMap = plc.ReadSlotMapFromSensor(e.LoadPortId);
                e87Manager.PerformSlotMapping(carrierId, slotMap);
                
                // 4. Report to MES
                var carrier = e87Manager.GetCarrier(carrierId);
                await mes.ReportCarrierArrival(carrier, e.LoadPortId);
                
                // 5. Send SECS event
                secsGem.SendEventReport(1001, new
                {
                    CarrierId = carrierId,
                    LoadPortId = e.LoadPortId
                });
                
                // 6. Log to database
                history.LogCarrierEvent(carrier, "ARRIVED", 
                    $"Loaded to {e.LoadPortId}");
            }
        };
    }
    
    public async Task ProcessCarrier(string carrierId)
    {
        var carrier = e87Manager.GetCarrier(carrierId);
        var loadPort = e87Manager.FindLoadPortByCarrier(carrierId);
        
        // 1. Open door (physical)
        plc.OpenDoor(loadPort.LoadPortId);
        e87Manager.OpenLoadPortDoor(loadPort.LoadPortId);
        
        // 2. Get recipe from MES
        var recipe = await mes.RequestProcessRecipe(carrierId);
        
        // 3. Process wafers (robot picks from carrier)
        // ... wafer processing logic ...
        
        // 4. Close door (physical)
        plc.CloseDoor(loadPort.LoadPortId);
        e87Manager.CloseLoadPortDoor(loadPort.LoadPortId);
        
        // 5. Report completion
        await mes.ReportProcessComplete(carrierId, new ProcessResult
        {
            WaferCount = carrier.GetOccupiedSlotCount(),
            Success = true
        });
    }
}
```

## Summary

This integration guide demonstrates how to connect E87 carrier management with:
- ✅ GEM300 complete suite
- ✅ SECS/GEM messaging (E30/E37)
- ✅ Substrate tracking (E90)
- ✅ Control job management (E94)
- ✅ MES systems
- ✅ PLC/hardware control
- ✅ RFID readers
- ✅ Database persistence

The modular design allows you to integrate any combination of these systems based on your specific requirements.
