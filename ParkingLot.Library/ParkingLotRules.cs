using System.Collections.Concurrent;

namespace ParkingLot.Library
{
    /// <summary>
    /// Defines business rules for parking lot entry and exit validation.
    /// Enforces authorization, capacity limits, and duplicate parking prevention.
    /// </summary>
    public static class ParkingLotRules
    {
        /// <summary>
        /// Maximum number of vehicles allowed in the parking lot simultaneously.
        /// </summary>
        public static readonly int MaxCapacity = 150;

        /// <summary>
        /// Determines if a vehicle can enter the parking lot.
        /// Requirements: authorized driver, valid license plate, car not already parked, space available.
        /// </summary>
        public static bool CanEnter(
            ConcurrentDictionary<string, byte> authorizedDrivers,
            ConcurrentDictionary<string, byte> parkedCars,
            string? driverId,
            string? licensePlate,
            bool inductiveSensor)
        {
            return driverId != null &&
                   licensePlate != null &&
                   inductiveSensor &&
                   authorizedDrivers.ContainsKey(driverId) &&
                   !parkedCars.ContainsKey(licensePlate) &&
                   parkedCars.Count < MaxCapacity;
        }

        /// <summary>
        /// Determines if a vehicle can exit the parking lot.
        /// Requirement: vehicle must be currently parked.
        /// </summary>
        public static bool CanExit(ConcurrentDictionary<string, byte> parkedCars, string? licensePlate, bool inductiveSensor)
        {
            return licensePlate != null && parkedCars.ContainsKey(licensePlate) && inductiveSensor;
        }
    }
}