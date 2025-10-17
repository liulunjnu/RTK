using System;
using System.Collections.Generic;
using System.Threading;
using SemiStandards.Common;
using SemiE39;
using SemiE84;
using SemiE87;
using SemiE90;
using SemiE94;
using SemiE116;

namespace SemiStandards.Demo
{
    /// <summary>
    /// 完整的SEMI标准集成演示
    /// 展示E39, E84, E87, E90, E94, E116如何协同工作
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   SEMI Standards Integration Demo (.NET Framework 4.7.2)     ║");
            Console.WriteLine("║   E39 + E84 + E87 + E90 + E94 + E116                         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // 初始化所有SEMI标准管理器
            var objectService = new ObjectService("EquipmentObjectService");  // E39
            var e84Handler = new E84Handler("LP1", E84Mode.Active);           // E84
            var e87Manager = new E87Manager("EQP-300mm-001");                 // E87
            var e90Tracker = new SubstrateTracker();                          // E90
            var e94Manager = new ControlJobManager();                         // E94
            var e116Tracker = new PerformanceTracker();                       // E116

            Console.WriteLine("✓ All SEMI standard managers initialized\n");

            // ===== 演示流程 =====
            
            // 第1步：E39 - 创建设备对象
            DemoE39ObjectServices(objectService);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第2步：E87 - 载体管理
            DemoE87CarrierManagement(e87Manager);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第3步：E84 - 载体交接
            DemoE84CarrierHandoff(e84Handler);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第4步：E90 - 基板跟踪
            DemoE90SubstrateTracking(e90Tracker, e87Manager);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第5步：E94 - 控制作业管理
            DemoE94ControlJob(e94Manager);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第6步：E116 - 性能跟踪
            DemoE116Performance(e116Tracker);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // 第7步：完整集成场景
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("COMPLETE INTEGRATION SCENARIO");
            Console.WriteLine(new string('=', 70));
            DemoCompleteIntegration(objectService, e84Handler, e87Manager, e90Tracker, e94Manager, e116Tracker);

            Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Demo Completed Successfully!                     ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void DemoE39ObjectServices(ObjectService objectService)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E39 - OBJECT SERVICES DEMO");
            Console.WriteLine(new string('=', 70));

            // 创建设备对象
            var equipment = objectService.CreateObject("EQP-001", "Equipment", new Dictionary<string, object>
            {
                { "Model", "300mm-Processor" },
                { "Manufacturer", "SEMI Corp" },
                { "InstallDate", DateTime.Now }
            });
            Console.WriteLine($"✓ Created: {equipment}");

            // 创建载体对象
            var carrier = objectService.CreateObject("CARRIER-001", "Carrier", new Dictionary<string, object>
            {
                { "Type", "FOUP" },
                { "Capacity", 25 }
            });
            Console.WriteLine($"✓ Created: {carrier}");

            // 修改对象属性
            objectService.ModifyObjectAttribute("EQP-001", "Model", "300mm-Processor-v2");
            Console.WriteLine("✓ Modified equipment model");

            // 查询对象
            var equipmentObjects = objectService.GetObjectsByType("Equipment");
            Console.WriteLine($"✓ Found {CountObjects(equipmentObjects)} equipment objects");
        }

        static void DemoE87CarrierManagement(E87Manager e87Manager)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E87 - CARRIER MANAGEMENT DEMO");
            Console.WriteLine(new string('=', 70));

            // 添加装载端口
            e87Manager.AddLoadPort("LP1");
            e87Manager.AddLoadPort("LP2");
            Console.WriteLine("✓ Added 2 Load Ports");

            // 注册载体
            var carrier = e87Manager.RegisterCarrier("CARRIER-001", 25);
            carrier.CarrierType = "FOUP";
            carrier.IdStatus = CarrierIdStatus.IdVerified;
            Console.WriteLine($"✓ Registered: {carrier}");

            // 装载载体
            e87Manager.LoadCarrierToPort("CARRIER-001", "LP1");
            Console.WriteLine("✓ Carrier loaded to LP1");

            // 更新槽位映射
            var slotMap = new Dictionary<int, SlotOccupancyStatus>
            {
                { 1, SlotOccupancyStatus.Occupied },
                { 2, SlotOccupancyStatus.Occupied },
                { 3, SlotOccupancyStatus.Occupied },
                { 4, SlotOccupancyStatus.Empty },
                { 5, SlotOccupancyStatus.Occupied }
            };
            carrier.UpdateSlotMap(slotMap);
            Console.WriteLine($"✓ Slot map updated: {carrier.GetOccupiedSlotCount()}/25 wafers");

            Console.WriteLine($"\n{e87Manager.GetStatus()}");
        }

        static void DemoE84CarrierHandoff(E84Handler e84Handler)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E84 - CARRIER HANDOFF DEMO");
            Console.WriteLine(new string('=', 70));

            Console.WriteLine("Starting Load Transfer...");
            e84Handler.StartLoad();
            
            // 模拟E84信号交换
            Console.WriteLine("\nSimulating E84 handshake signals...");
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(200);
                
                var signals = new E84Signals();
                switch (i)
                {
                    case 0:
                        signals.VALID = true;
                        Console.WriteLine("  → VALID signal received");
                        break;
                    case 1:
                        signals.TR_REQ = true;
                        Console.WriteLine("  → TR_REQ signal received");
                        break;
                    case 2:
                        Console.WriteLine("  → Transfer in progress...");
                        break;
                    case 3:
                        signals.CS_0 = true;
                        Console.WriteLine("  → CS_0 signal (carrier secured)");
                        break;
                    case 4:
                        signals.CONT = true;
                        Console.WriteLine("  → CONT signal (continue)");
                        break;
                }
                
                e84Handler.UpdateInputs(signals);
            }

            Console.WriteLine($"\n✓ Transfer completed");
            Console.WriteLine($"Final State: {e84Handler.CurrentState}");
        }

        static void DemoE90SubstrateTracking(SubstrateTracker e90Tracker, E87Manager e87Manager)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E90 - SUBSTRATE TRACKING DEMO");
            Console.WriteLine(new string('=', 70));

            // 注册基板
            var substrateIds = new List<string> { "WAFER-001", "WAFER-002", "WAFER-003" };
            foreach (var subId in substrateIds)
            {
                var substrate = e90Tracker.RegisterSubstrate(subId, SubstrateType.ProductWafer);
                e90Tracker.AssociateWithCarrier(subId, "CARRIER-001", substrateIds.IndexOf(subId) + 1);
                Console.WriteLine($"✓ Registered: {substrate}");
            }

            // 模拟处理流程
            Console.WriteLine("\nProcessing substrates...");
            foreach (var subId in substrateIds)
            {
                e90Tracker.UpdateLocation(subId, SubstrateState.AtStation, "ProcessChamber-1", "Chamber1");
                e90Tracker.StartProcessing(subId, "Chamber1", "RECIPE-001");
                Console.WriteLine($"  → {subId} processing started");
                
                Thread.Sleep(100);
                
                e90Tracker.CompleteProcessing(subId, SubstrateProcessResult.Success);
                Console.WriteLine($"  → {subId} processing completed");
            }

            var stats = e90Tracker.GetStatistics();
            Console.WriteLine($"\n✓ Statistics: Total={stats["Total"]}, Success={stats["Success"]}, Completed={stats["Completed"]}");
        }

        static void DemoE94ControlJob(ControlJobManager e94Manager)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E94 - CONTROL JOB MANAGEMENT DEMO");
            Console.WriteLine(new string('=', 70));

            // 创建控制作业
            var carrierIds = new List<string> { "CARRIER-001" };
            var substratesByCarrier = new Dictionary<string, List<string>>
            {
                { "CARRIER-001", new List<string> { "WAFER-001", "WAFER-002", "WAFER-003" } }
            };

            var controlJob = e94Manager.CreateControlJob("CJ-001", "RECIPE-001", carrierIds, substratesByCarrier);
            Console.WriteLine($"✓ Created: {controlJob}");

            // 开始控制作业
            e94Manager.StartControlJob("CJ-001");
            Console.WriteLine("✓ Control Job started");

            // 处理每个基板
            foreach (var subId in substratesByCarrier["CARRIER-001"])
            {
                e94Manager.StartProcessJob("CJ-001", subId, "Chamber1");
                Console.WriteLine($"  → Processing {subId}");
                Thread.Sleep(100);
                e94Manager.CompleteProcessJob("CJ-001", subId, "SUCCESS");
                Console.WriteLine($"  → Completed {subId}");
            }

            Console.WriteLine($"\n✓ {controlJob}");
        }

        static void DemoE116Performance(PerformanceTracker e116Tracker)
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("E116 - PERFORMANCE TRACKING DEMO");
            Console.WriteLine(new string('=', 70));

            // 开始性能跟踪
            e116Tracker.StartOperational();
            e116Tracker.StartProductive();
            Console.WriteLine("✓ Performance tracking started");

            // 模拟处理
            Console.WriteLine("\nSimulating substrate processing...");
            for (int i = 1; i <= 5; i++)
            {
                Thread.Sleep(100);
                e116Tracker.RecordSubstrateProcessed(true);
                Console.WriteLine($"  → Substrate {i} processed");
            }

            // 模拟停机
            e116Tracker.StopProductive();
            e116Tracker.RecordDowntime(30, false);
            Console.WriteLine("  → Downtime recorded (30 sec)");

            // 生成报告
            e116Tracker.StopOperational();
            Console.WriteLine("\n" + e116Tracker.GenerateReport());
        }

        static void DemoCompleteIntegration(ObjectService e39, E84Handler e84, E87Manager e87, 
            SubstrateTracker e90, ControlJobManager e94, PerformanceTracker e116)
        {
            Console.WriteLine("\nExecuting complete integrated workflow...\n");

            // 步骤1：创建对象 (E39)
            Console.WriteLine("Step 1: Creating equipment objects (E39)...");
            var eqp = e39.CreateObject("EQP-INTEGRATED", "Equipment", new Dictionary<string, object>
            {
                { "Model", "GEM300-Processor" }
            });

            // 步骤2：载体管理 (E87)
            Console.WriteLine("Step 2: Managing carrier (E87)...");
            e87.AddLoadPort("LP-INT");
            var carrier = e87.RegisterCarrier("CARRIER-INT", 25);

            // 步骤3：载体交接 (E84)
            Console.WriteLine("Step 3: Carrier handoff (E84)...");
            e84.Reset();
            e84.StartLoad();

            // 步骤4：基板跟踪 (E90)
            Console.WriteLine("Step 4: Tracking substrates (E90)...");
            var substrates = new List<string> { "W-INT-1", "W-INT-2" };
            foreach (var sub in substrates)
            {
                e90.RegisterSubstrate(sub);
                e90.AssociateWithCarrier(sub, "CARRIER-INT", substrates.IndexOf(sub) + 1);
            }

            // 步骤5：控制作业 (E94)
            Console.WriteLine("Step 5: Creating control job (E94)...");
            var cj = e94.CreateControlJob("CJ-INT", "RECIPE-INT", 
                new List<string> { "CARRIER-INT" },
                new Dictionary<string, List<string>> { { "CARRIER-INT", substrates } });
            e94.StartControlJob("CJ-INT");

            // 步骤6：性能跟踪 (E116)
            Console.WriteLine("Step 6: Tracking performance (E116)...");
            e116.StartOperational();
            e116.StartProductive();

            // 执行处理
            Console.WriteLine("\nProcessing wafers...");
            foreach (var sub in substrates)
            {
                Thread.Sleep(50);
                e90.StartProcessing(sub, "Chamber1", "RECIPE-INT");
                e94.StartProcessJob("CJ-INT", sub, "Chamber1");
                e116.RecordSubstrateProcessed(true);
                
                Thread.Sleep(50);
                e90.CompleteProcessing(sub, SubstrateProcessResult.Success);
                e94.CompleteProcessJob("CJ-INT", sub, "SUCCESS");
                
                Console.WriteLine($"  ✓ {sub} completed");
            }

            e116.StopProductive();
            e116.StopOperational();

            // 总结
            Console.WriteLine("\n" + new string('-', 70));
            Console.WriteLine("INTEGRATION SUMMARY:");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"E39 Objects: {e39.GetStatistics()}");
            Console.WriteLine($"E87 Status: {e87.GetStatus()}");
            Console.WriteLine($"E84 State: {e84.CurrentState}");
            Console.WriteLine($"E90 Substrates: {e90.GetStatistics()["Total"]} tracked");
            Console.WriteLine($"E94 Control Job: {cj.State}");
            Console.WriteLine($"E116 OEE: {e116.CurrentMetrics.OEE:F2}%");
            Console.WriteLine(new string('-', 70));
        }

        static int CountObjects(IEnumerable<SemiObject> objects)
        {
            int count = 0;
            foreach (var obj in objects)
            {
                count++;
            }
            return count;
        }
    }
}
