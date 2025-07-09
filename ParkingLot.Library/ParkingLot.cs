using System.Collections.Concurrent;

namespace ParkingLot.Library
{
    public class ParkingLot : IParkingLot
    {
        /// <summary>
        /// Set of driver IDs authorized to use the parking lot.
        /// </summary>
        private readonly ConcurrentDictionary<string, byte> _authorizedDrivers = new();

        /// <summary>
        /// Set of license plates for vehicles currently parked in the lot.
        /// Shared between entry and exit gates to maintain accurate occupancy.
        /// </summary>
        private readonly ConcurrentDictionary<string, byte> _parkedCarLicenses = new();

        public IParkingLotGate InitEntryGate()
        {
            return new ParkingLotGate(_authorizedDrivers, _parkedCarLicenses, true);
        }

        public IParkingLotGate InitExitGate()
        {
            return new ParkingLotGate(_authorizedDrivers, _parkedCarLicenses, false);
        }

        public void AddAuthorizedDriver(string driverId)
        {
            _authorizedDrivers.TryAdd(driverId,0);
        }

        public void RemoveAuthorizedDriver(string driverId)
        {
            _authorizedDrivers.TryRemove(driverId, out _);
        }
    }
}