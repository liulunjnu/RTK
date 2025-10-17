using System;
using System.Collections.Generic;

namespace SemiE87
{
    /// <summary>
    /// SEMI E87 协议示例程序
    /// 演示载体管理的完整流程
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SEMI E87 Carrier Management System Demo ===\n");

            // 1. 创建E87管理器
            var e87Manager = new E87Manager("EQP-12345");
            e87Manager.IsOnline = true;

            // 订阅事件
            e87Manager.CarrierRegistered += (sender, e) =>
                Console.WriteLine($"[EVENT] Carrier registered: {e.Carrier.CarrierId}");
            
            e87Manager.CarrierIdRead += (sender, e) =>
                Console.WriteLine($"[EVENT] Carrier ID read: {e.Carrier.CarrierId}");
            
            e87Manager.LoadPortStateChanged += (sender, e) =>
                Console.WriteLine($"[EVENT] LoadPort {e.LoadPortId} state changed");

            // 2. 添加装载端口
            Console.WriteLine("Step 1: Adding Load Ports...");
            e87Manager.AddLoadPort("LP1", "Front-Left");
            e87Manager.AddLoadPort("LP2", "Front-Right");
            Console.WriteLine("✓ Added 2 Load Ports\n");

            // 3. 注册载体
            Console.WriteLine("Step 2: Registering Carriers...");
            var carrier1 = e87Manager.RegisterCarrier("CARRIER-001", 25, "FOUP");
            var carrier2 = e87Manager.RegisterCarrier("CARRIER-002", 25, "FOUP");
            Console.WriteLine("✓ Registered 2 Carriers\n");

            // 4. 模拟载体到达场景
            Console.WriteLine("Step 3: Carrier Arrival Scenario...");
            
            // 读取载体ID（模拟RFID读取）
            Console.WriteLine("  Reading Carrier ID at LP1...");
            e87Manager.ReadCarrierId("LP1", "CARRIER-001");
            
            // 验证载体ID
            Console.WriteLine("  Verifying Carrier ID...");
            bool verified = e87Manager.VerifyCarrierId("CARRIER-001", "CARRIER-001");
            Console.WriteLine($"  Verification result: {(verified ? "PASS ✓" : "FAIL ✗")}");

            // 装载载体到端口
            Console.WriteLine("  Loading Carrier to LP1...");
            e87Manager.LoadCarrierToPort("CARRIER-001", "LP1");
            Console.WriteLine("✓ Carrier loaded successfully\n");

            // 5. 槽位映射
            Console.WriteLine("Step 4: Performing Slot Mapping...");
            var slotMap = new Dictionary<int, SlotOccupancyStatus>
            {
                { 1, SlotOccupancyStatus.Occupied },
                { 2, SlotOccupancyStatus.Occupied },
                { 3, SlotOccupancyStatus.Empty },
                { 4, SlotOccupancyStatus.Occupied },
                { 5, SlotOccupancyStatus.Occupied }
                // 其余槽位保持Unknown状态
            };
            
            e87Manager.PerformSlotMapping("CARRIER-001", slotMap);
            Console.WriteLine($"  Slot Map: {carrier1.GetOccupiedSlotCount()} wafers detected");
            Console.WriteLine("✓ Slot mapping completed\n");

            // 6. 开门访问晶圆
            Console.WriteLine("Step 5: Accessing Wafers...");
            var lp1 = e87Manager.GetLoadPort("LP1");
            
            Console.WriteLine("  Opening door...");
            e87Manager.OpenLoadPortDoor("LP1");
            Console.WriteLine($"  Door state: {lp1.DoorState}");
            
            Console.WriteLine("  [Simulating wafer processing...]");
            System.Threading.Thread.Sleep(500);
            
            Console.WriteLine("  Closing door...");
            e87Manager.CloseLoadPortDoor("LP1");
            Console.WriteLine($"  Door state: {lp1.DoorState}");
            Console.WriteLine("✓ Wafer access completed\n");

            // 7. 卸载载体
            Console.WriteLine("Step 6: Unloading Carrier...");
            lp1.PrepareForRelease();
            e87Manager.UnloadCarrierFromPort("LP1");
            Console.WriteLine("✓ Carrier unloaded successfully\n");

            // 8. 演示第二个载体流程
            Console.WriteLine("Step 7: Processing Second Carrier...");
            e87Manager.ReadCarrierId("LP2", "CARRIER-002");
            e87Manager.VerifyCarrierId("CARRIER-002", "CARRIER-002");
            e87Manager.LoadCarrierToPort("CARRIER-002", "LP2");
            e87Manager.SetCarrierAccessMode("CARRIER-002", CarrierAccessMode.Auto);
            Console.WriteLine("✓ Second carrier loaded to LP2\n");

            // 9. 显示系统状态
            Console.WriteLine("Step 8: System Status Report");
            Console.WriteLine(e87Manager.GetSystemStatus());

            // 10. 显示详细信息
            Console.WriteLine("\n=== Detailed Carrier Information ===");
            foreach (var carrier in e87Manager.GetAllCarriers())
            {
                Console.WriteLine($"\nCarrier: {carrier.CarrierId}");
                Console.WriteLine($"  Type: {carrier.CarrierType}");
                Console.WriteLine($"  Location: {carrier.Location}");
                Console.WriteLine($"  Association: {carrier.AssociationState}");
                Console.WriteLine($"  Access Mode: {carrier.AccessMode}");
                Console.WriteLine($"  Slot Map Status: {carrier.SlotMapStatus}");
                Console.WriteLine($"  Occupied Slots: {carrier.GetOccupiedSlotCount()}/{carrier.SlotCount}");
                
                if (carrier.SlotMapStatus == SlotMapStatus.Verified)
                {
                    Console.WriteLine("  Slot Details:");
                    for (int i = 1; i <= 5; i++) // 只显示前5个槽位作为示例
                    {
                        var slot = carrier.SlotMap[i - 1];
                        if (slot.OccupancyStatus != SlotOccupancyStatus.Unknown)
                        {
                            Console.WriteLine($"    {slot}");
                        }
                    }
                }
            }

            Console.WriteLine("\n=== Demo Completed Successfully ===");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
