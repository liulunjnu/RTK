using System;
using System.Threading;
using Gem300.Framework.Common;
using Gem300.Framework.E39;
using Gem300.Framework.E84;
using Gem300.Framework.E87;
using Gem300.Framework.E90;
using Gem300.Framework.E94;
using Gem300.Framework.E116;

namespace Gem300.Sim.Framework
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("GEM300 (net48) simulator starting...");

            var e84 = new CarrierHandoffStateMachine();
            var e87 = new CarrierManager();
            var e90 = new SubstrateTracker();
            var e94 = new ControlJobManager();
            var e116 = new EquipmentPerformanceTracker();

            e84.OnStateChanged += s => Console.WriteLine("E84 state: " + s);
            Console.WriteLine("-- E84 demo");
            e84.Handle(E84Signal.EquipmentReady);
            e84.Handle(E84Signal.DockRequest);
            e84.Handle(E84Signal.Complete);
            e84.Handle(E84Signal.HostReadyToUnload);
            e84.Handle(E84Signal.UndockRequest);
            e84.Handle(E84Signal.Complete);

            Console.WriteLine("-- E87/E90 demo");
            var cId = new CarrierId("C001");
            var pId = new PortId("P1");
            e87.RegisterCarrier(cId, pId, 5);
            e90.RegisterInCarrier(new SubstrateId("W001"), cId, new Slot(1));
            e90.MoveToModule(new SubstrateId("W001"), "PM1");
            e90.ReturnToCarrier(new SubstrateId("W001"), cId, new Slot(2));

            Console.WriteLine("-- E94 demo");
            e94.Create("CJ-001");
            e94.BindCarrier("CJ-001", cId);
            e94.Start("CJ-001");
            Thread.Sleep(200);
            e94.Complete("CJ-001");

            Console.WriteLine("-- E116 demo");
            e116.SetCategory("Productive");
            Thread.Sleep(100);
            e116.Tick();
            e116.SetCategory("SDT");
            Thread.Sleep(50);
            e116.Tick();
            var m = e116.Snapshot();
            Console.WriteLine(string.Format("OEE ~ {0:P2}, Productive={1}ms, SDT={2}ms", m.OEE, m.ProductiveTime.TotalMilliseconds, m.ScheduledDownTime.TotalMilliseconds));

            Console.WriteLine("Simulation done.");
        }
    }
}
