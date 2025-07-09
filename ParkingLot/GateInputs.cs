using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot
{
    internal class GateInputs : IParkingLotGateInputs
    {
        public bool GateFullyOpen { get; set; }

        public bool GateFullyClosed { get; set; }

        public bool SafetySensor { get; set; }

        public bool EStop { get; set; }

        public string? DriverId { get; set; }

        public bool InductiveSensor { get; set; }

        public string? LicensePlate { get; set; }
    }
}
