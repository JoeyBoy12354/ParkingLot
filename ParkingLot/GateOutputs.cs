using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot
{
    internal class GateOutputs : IParkingLotGateOutputs
    {
        public bool OpenGate { get; set; }

        public bool CloseGate { get; set; }

        public bool GreenLight { get; set; }

        public bool YellowLight { get; set; }

        public bool RedLight { get; set; }
    }
}
