namespace ParkingLot
{
    public interface IParkingLotGate
    {
        /// <summary>
        /// Runs the control logic for the parking lot gate
        /// </summary>
        /// <param name="inputs">Inputs to be used on the current logic cycle</param>
        /// <param name="outputs">The outputs that are set at the end of the logic cycle</param>
        void RunCycle(IParkingLotGateInputs inputs, IParkingLotGateOutputs outputs);
    }
}
