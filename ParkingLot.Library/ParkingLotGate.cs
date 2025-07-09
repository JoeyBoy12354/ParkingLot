using ParkingLot.Library.Enums;
using System.Collections.Concurrent;

namespace ParkingLot.Library
{
    /// <summary>
    /// Manages the operation of a parking lot gate (entry or exit) including validation,
    /// safety sensors, and gate state transitions.
    /// </summary>
    public class ParkingLotGate : IParkingLotGate
    {
        private readonly ConcurrentDictionary<string, byte> _authorizedDrivers;
        private readonly ConcurrentDictionary<string, byte> _parkedCarLicenses;
        private GateState _currentState = GateState.Idle;

        /// <summary>
        /// Set true for an enterance and false for an exit
        /// </summary>
        readonly bool _isEntry;

        public ParkingLotGate (ConcurrentDictionary<string, byte> authorizedDrivers, ConcurrentDictionary<string, byte> parkedCarLicenses, bool isEntry)
        {
            _authorizedDrivers = authorizedDrivers;
            _parkedCarLicenses = parkedCarLicenses;
            _isEntry = isEntry;
        }

        public void RunCycle(IParkingLotGateInputs inputs, IParkingLotGateOutputs outputs)
        {
            if (inputs.EStop)
            {
                _currentState = GateState.Emergency;
                SetLightState(outputs, red: true);
                outputs.OpenGate = false;
                outputs.CloseGate = false;
                return;
            }

            switch (_currentState)
            {
                case GateState.Idle:
                    SetLightState(outputs, red: true);

                    // This is set since the gate will not always input that it is fully closed when in idle state
                    if (!inputs.GateFullyClosed)
                        outputs.CloseGate = true;

                    if (inputs.InductiveSensor)
                    {
                        _currentState = GateState.Validation;
                    }
                    break;

                case GateState.Validation:
                    // Validate entering vehicle - update parked cars list immediately upon validation
                    if (_isEntry && 
                        ParkingLotRules.CanEnter(_authorizedDrivers, 
                            _parkedCarLicenses, 
                            inputs.DriverId, 
                            inputs.LicensePlate, 
                            inputs.InductiveSensor)
                        )
                    {
                        SetLightState(outputs, yellow: true);
                        outputs.OpenGate = true;
                        _parkedCarLicenses.TryAdd(inputs.LicensePlate, 0);
                        _currentState = GateState.OpeningGate;
                    }

                    // Validate exiting vehicle
                    else if (!_isEntry && ParkingLotRules.CanExit(_parkedCarLicenses, inputs.LicensePlate, inputs.InductiveSensor))
                    {
                        SetLightState(outputs, yellow: true);
                        outputs.OpenGate = true;
                        _parkedCarLicenses.TryRemove(inputs.LicensePlate, out _);
                        _currentState = GateState.OpeningGate;
                    }

                    // Invalid
                    else
                    {
                        _currentState = GateState.Idle;
                    }
                    break;

                case GateState.OpeningGate:
                    if (inputs.GateFullyOpen)
                    {
                        SetLightState(outputs, green: true);
                        _currentState = GateState.OpenGateWaitForCar;
                    }
                    else
                    {
                        outputs.OpenGate = true;
                        SetLightState(outputs, yellow: true);
                    }
                    break;

                case GateState.OpenGateWaitForCar:
                    // Wait for car to completely clear the gate area (both sensors must be clear)
                    if (!inputs.SafetySensor && !inputs.InductiveSensor)
                    {
                        SetLightState(outputs, yellow: true);
                        outputs.CloseGate = true;
                        _currentState = GateState.ClosingGate;
                    } 
                    else
                    {
                        outputs.OpenGate = true;
                        SetLightState(outputs, green: true);
                    }
                    break;

                case GateState.ClosingGate:
                    if (inputs.GateFullyClosed)
                    {
                        SetLightState(outputs, red: true);
                        _currentState = GateState.Idle;
                    }
                    else
                    {
                        outputs.CloseGate = true;
                        SetLightState(outputs, yellow: true);
                    }
                    break;

                case GateState.Emergency:
                    outputs.OpenGate = false;
                    outputs.CloseGate = false;
                    SetLightState(outputs, red: true);
                    if (!inputs.EStop)
                    {
                        _currentState = GateState.Idle;
                    }
                    break;
            }  
        }

        private static void SetLightState(IParkingLotGateOutputs outputs, bool red = false, bool yellow = false, bool green = false)
        {
            outputs.RedLight = red;
            outputs.YellowLight = yellow;
            outputs.GreenLight = green;
        }
    }
}
