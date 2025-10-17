using System;
using System.Collections.Generic;
using GEM300;
using GEM300.Events;

namespace GEM300.Examples
{
    /// <summary>
    /// Example usage of GEM300 event system
    /// </summary>
    public class UsageExample
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("GEM300 Event System - Usage Examples\n");
            
            // Get the event manager instance
            var eventManager = Gem300EventManager.Instance;
            
            // Example 1: Subscribe to E39 events
            Console.WriteLine("=== Example 1: E39 Processing State Change ===");
            eventManager.Subscribe("E39", (gemEvent) =>
            {
                var e39 = gemEvent as E39ProcessingStateChange;
                Console.WriteLine($"[E39] State changed from {e39.PreviousState} to {e39.CurrentState}");
                Console.WriteLine($"     Equipment: {e39.EquipmentId}, Reason: {e39.Reason}");
            });
            
            // Publish E39 event
            var e39Event = new E39ProcessingStateChange(
                equipmentId: "EQP-001",
                previousState: E39ProcessingStateChange.ProcessingState.IDLE,
                currentState: E39ProcessingStateChange.ProcessingState.EXECUTING,
                reason: "Recipe started"
            );
            eventManager.PublishEvent(e39Event);
            Console.WriteLine($"Serialized: {e39Event.Serialize()}\n");
            
            // Example 2: E84 Material Received
            Console.WriteLine("=== Example 2: E84 Material Received ===");
            eventManager.Subscribe("E84", (gemEvent) =>
            {
                var e84 = gemEvent as E84MaterialReceived;
                Console.WriteLine($"[E84] Material received: {e84.MaterialId}");
                Console.WriteLine($"     Port: {e84.PortId}, Carrier: {e84.CarrierId}, Slot: {e84.SlotNumber}");
            });
            
            var e84Event = new E84MaterialReceived(
                equipmentId: "EQP-001",
                materialId: "LOT-12345",
                portId: "PORT-1",
                carrierId: "CARR-001",
                slotNumber: 1,
                locationCode: "LOAD_LOCK_1"
            );
            eventManager.PublishEvent(e84Event);
            
            // Example 3: E87 Material Removed
            Console.WriteLine("\n=== Example 3: E87 Material Removed ===");
            eventManager.Subscribe("E87", (gemEvent) =>
            {
                var e87 = gemEvent as E87MaterialRemoved;
                Console.WriteLine($"[E87] Material removed: {e87.MaterialId}");
                Console.WriteLine($"     Result: {e87.ProcessingResult}");
            });
            
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
            
            // Example 4: E90 Spooling Activity
            Console.WriteLine("\n=== Example 4: E90 Spooling Activity ===");
            eventManager.Subscribe("E90", (gemEvent) =>
            {
                var e90 = gemEvent as E90SpoolingActivity;
                Console.WriteLine($"[E90] Spooling {e90.ActivityType}: {e90.DataType}");
                Console.WriteLine($"     Stream: {e90.SpoolStreamId}, Records: {e90.RecordCount}");
            });
            
            var e90Event = new E90SpoolingActivity(
                equipmentId: "EQP-001",
                spoolStreamId: "SPOOL-001",
                activityType: E90SpoolingActivity.SpoolingActivityType.COMPLETE,
                dataType: "ProcessData",
                recordCount: 1250,
                destination: "/data/spool/process_20231017.dat"
            );
            eventManager.PublishEvent(e90Event);
            
            // Example 5: E94 Control Job Status
            Console.WriteLine("\n=== Example 5: E94 Control Job Status ===");
            eventManager.Subscribe("E94", (gemEvent) =>
            {
                var e94 = gemEvent as E94ControlJobStatus;
                Console.WriteLine($"[E94] Control Job {e94.ControlJobId}: {e94.PreviousStatus} → {e94.CurrentStatus}");
                Console.WriteLine($"     Recipe: {e94.ProcessProgramId}, Materials: {string.Join(", ", e94.MaterialIds)}");
            });
            
            var e94Event = new E94ControlJobStatus(
                equipmentId: "EQP-001",
                controlJobId: "CJ-2023-1017-001",
                previousStatus: E94ControlJobStatus.ControlJobStatusType.WAITING,
                currentStatus: E94ControlJobStatus.ControlJobStatusType.EXECUTING,
                processProgramId: "RECIPE-ABC-123",
                materialIds: new List<string> { "LOT-12345", "LOT-12346" },
                statusReason: "All resources available"
            );
            eventManager.PublishEvent(e94Event);
            
            // Example 6: E116 Machine Data Collection
            Console.WriteLine("\n=== Example 6: E116 Machine Data Collection ===");
            eventManager.Subscribe("E116", (gemEvent) =>
            {
                var e116 = gemEvent as E116MachineDataCollection;
                Console.WriteLine($"[E116] Data Collection Report: {e116.ReportId}");
                Console.WriteLine($"     Category: {e116.DataCategory}, Items: {e116.DataItems.Count}");
                foreach (var item in e116.DataItems)
                {
                    Console.WriteLine($"       - {item.DataName}: {item.Value} {item.Unit}");
                }
            });
            
            var e116Event = new E116MachineDataCollection(
                equipmentId: "EQP-001",
                reportId: "RPT-001",
                collectionEventId: "E116",
                dataCategory: "ProcessData",
                materialId: "LOT-12345",
                processProgramId: "RECIPE-ABC-123"
            );
            e116Event.AddDataItem("TEMP_CHAMBER", "Chamber Temperature", 350.5, "°C");
            e116Event.AddDataItem("PRESSURE_VAC", "Vacuum Pressure", 1.2e-5, "Torr");
            e116Event.AddDataItem("POWER_RF", "RF Power", 1500, "W");
            e116Event.AddDataItem("FLOW_N2", "N2 Flow Rate", 200, "sccm");
            eventManager.PublishEvent(e116Event);
            
            // Example 7: Query event history
            Console.WriteLine("\n=== Example 7: Event History ===");
            var allEvents = eventManager.GetEventHistory();
            Console.WriteLine($"Total events in history: {allEvents.Count}");
            
            var e39Events = eventManager.GetEventHistory("E39");
            Console.WriteLine($"E39 events: {e39Events.Count}");
            
            // Example 8: Subscribe to all events
            Console.WriteLine("\n=== Example 8: Global Event Handler ===");
            eventManager.OnEventPublished += (gemEvent) =>
            {
                Console.WriteLine($"[GLOBAL] Event published: {gemEvent.EventId} - {gemEvent.EventName} at {gemEvent.Timestamp}");
            };
            
            // Trigger another event to demonstrate global handler
            var testEvent = new E39ProcessingStateChange(
                equipmentId: "EQP-002",
                previousState: E39ProcessingStateChange.ProcessingState.EXECUTING,
                currentState: E39ProcessingStateChange.ProcessingState.COMPLETE,
                reason: "Process finished"
            );
            eventManager.PublishEvent(testEvent);
            
            Console.WriteLine("\n=== All Examples Completed ===");
        }
    }
}
