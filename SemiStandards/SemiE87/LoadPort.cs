using System;

namespace SemiE87
{
    public class LoadPort
    {
        public string LoadPortId { get; set; }
        public LoadPortState State { get; set; }
        public LoadPortTransferState TransferState { get; set; }
        public Carrier AssociatedCarrier { get; set; }
        public bool IsEnabled { get; set; }

        public LoadPort(string loadPortId)
        {
            LoadPortId = loadPortId;
            State = LoadPortState.Empty;
            TransferState = LoadPortTransferState.None;
            IsEnabled = true;
        }

        public bool LoadCarrier(Carrier carrier)
        {
            if (State != LoadPortState.Empty)
                throw new InvalidOperationException("LoadPort is not empty");

            State = LoadPortState.Loading;
            TransferState = LoadPortTransferState.Transferring;
            
            AssociatedCarrier = carrier;
            carrier.AssociateWithLoadPort(LoadPortId);
            
            State = LoadPortState.Loaded;
            TransferState = LoadPortTransferState.TransferComplete;
            
            return true;
        }

        public bool UnloadCarrier()
        {
            if (AssociatedCarrier == null)
                throw new InvalidOperationException("No carrier at LoadPort");

            State = LoadPortState.Unloading;
            TransferState = LoadPortTransferState.Transferring;
            
            AssociatedCarrier.Location = CarrierLocation.NotAtEquipment;
            AssociatedCarrier.AssociatedLoadPortId = null;
            AssociatedCarrier = null;
            
            State = LoadPortState.Empty;
            TransferState = LoadPortTransferState.None;
            
            return true;
        }

        public override string ToString()
        {
            return string.Format("LoadPort[{0}] State={1}, Carrier={2}", 
                LoadPortId, State, AssociatedCarrier != null ? AssociatedCarrier.CarrierId : "None");
        }
    }
}
