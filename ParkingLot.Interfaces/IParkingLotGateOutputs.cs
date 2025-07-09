
namespace ParkingLot
{
    /// <summary>
    /// Contains the outputs that are controlled by a parking lot gate
    /// </summary>
    public interface IParkingLotGateOutputs
    {
        /// <summary>
        /// The gate will open when set to true
        /// </summary>
        bool OpenGate { set; }
        /// <summary>
        /// The gate Will close when set to true
        /// </summary>
        bool CloseGate { set; }
        /// <summary>
        /// The green traffic light will be turned on when set to true
        /// </summary>
        bool GreenLight { set; }
        /// <summary>
        /// The yellow traffic light will be turned on when set to true
        /// </summary>
        bool YellowLight { set; }
        /// <summary>
        /// The red traffic light will be turned on when set to true
        /// </summary>
        bool RedLight { set; }
    }
}
