namespace ParkingLot
{
    /// <summary>
    /// Contains the inputs that are read by a parking lot gate
    /// </summary>
    public interface IParkingLotGateInputs
    {
        /// <summary>
        /// Indicates that the gate is fully open, true when fully open
        /// </summary>
        bool GateFullyOpen { get; }
        /// <summary>
        /// Indicates that the gate is fully closed, true when fully closed
        /// </summary>
        bool GateFullyClosed { get; }
        /// <summary>
        /// Indicates that there is an object below the gate, true when object detected
        /// </summary>
        bool SafetySensor { get; }
        /// <summary>
        /// Emergency stop button, true when emergency stopped
        /// </summary>
        bool EStop { get; }
        /// <summary>
        /// Contains the driver ID when read by the card reader or null if nothing is read
        /// </summary>
        string DriverId { get; }
        /// <summary>
        /// Reading from inductive sensor that detects when a car is present in front of the gate
        /// </summary>
		bool InductiveSensor { get; }
        /// <summary>
        /// Contains a license plate that is read by the license plate reader or null if nothing is read
        /// </summary>
        string LicensePlate { get; } 
    }
}
