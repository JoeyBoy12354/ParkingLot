using System;

namespace ParkingLot
{
    internal class Program
    {
        static IParkingLot _parkingLot;
        static IParkingLotGate _entryGate;
        static IParkingLotGate _exitGate;

        // We can use inputs as our simulation state
        static GateInputs _entryInputs;
        static GateInputs _exitInputs;

        static Program()
        {
            _parkingLot = new Library.ParkingLot();
            _entryGate = _parkingLot.InitEntryGate();
            _exitGate = _parkingLot.InitExitGate();

            _entryInputs = new GateInputs();
            _exitInputs = new GateInputs();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("\n--- Original Test ---");
            ResetSystem();
            OriginalTest();

            Console.WriteLine("\n--- Car Park Is Full Test ---");
            ResetSystem();
            CarParkIsFullTest();

            Console.WriteLine("\n--- Unauthorized Driver Test ---");
            ResetSystem();
            DriverIsInvalidTest();

            Console.WriteLine("\n--- Emergency Stop Test ---");
            ResetSystem();
            EmergencyStopTest();

            Console.WriteLine("\n--- Double Entry Test ---");
            ResetSystem();
            DoubleEntryTest();

            Console.WriteLine("\n--- Invalid Exit Test ---");
            ResetSystem();
            InvalidExitTest();

            Console.WriteLine("\n--- Safety Sensor Block Test ---");
            ResetSystem();
            SafetySensorBlocksCloseTest();
        }


        static void SimulateCarDrivingThroughGate(IParkingLotGate gate, GateInputs inputs)
        {
            // Car first triggers the inductive sensor
            //_exitInputs.InductiveSensor = true; (I believe this was a mistake)
            inputs.InductiveSensor = true; // (I believe this was the intention)

            // Gate should open, wait for it
            GateOutputs outputs;
            while (true)
            {
                outputs = SimulateCycle(gate, inputs);

                if (outputs.GreenLight)
                {
                    break;
                }

                Console.WriteLine($"{gate}: Waiting for green light");
            }

            Console.WriteLine($"{gate}: Green light, driving through gate");

            // Start driving in, safety sensor is triggered first
            inputs.SafetySensor = true;

            SimulateCycle(gate, inputs);

            // As car continues it exits the inductive sensor area and the readers are cleared
            inputs.InductiveSensor = false;
            inputs.LicensePlate = null;
            inputs.DriverId = null;

            SimulateCycle(gate, inputs);

            // Once fully through the safety sensor triggers off as well
            inputs.SafetySensor = false;

            SimulateCycle(gate, inputs);

            Console.WriteLine($"{gate}: Drove through gate");

            // Gate should be closed automatically after fully through
            while (!inputs.GateFullyClosed)
            {
                Console.WriteLine($"{gate}: Waiting for gate to be closed");

                outputs = SimulateCycle(gate, inputs);
            }
        }

        static GateOutputs SimulateCycle(IParkingLotGate gate, GateInputs inputs)
        {
            GateOutputs outputs = new GateOutputs();

            gate.RunCycle(inputs, outputs);
            
            // Handle opening the gate
            if (outputs.OpenGate)
            {
                if (inputs.GateFullyClosed)
                {
                    Console.WriteLine($"{gate}: Opening");
                    inputs.GateFullyClosed = false;
                    inputs.GateFullyOpen = false;
                }
                else
                {
                    Console.WriteLine($"{gate}: Fully open");
                    inputs.GateFullyOpen = true;
                }
            }

            // Handle closing the gate
            if (outputs.CloseGate)
            {
                if (inputs.GateFullyOpen)
                {
                    Console.WriteLine($"{gate}: Closing");
                    inputs.GateFullyClosed = false;
                    inputs.GateFullyOpen = false;
                }
                else
                {
                    Console.WriteLine($"{gate}: Fully closed");
                    inputs.GateFullyClosed = true;
                }
            }

            return outputs;
        }

        static void ResetSystem()
        {
            _parkingLot = new Library.ParkingLot();
            _entryGate = _parkingLot.InitEntryGate();
            _exitGate = _parkingLot.InitExitGate();

            _entryInputs = new GateInputs();
            _exitInputs = new GateInputs();
        }

        static void OriginalTest()
        {
            // Define a driver and a car
            string driver1Id = "Driver1";
            string car1LicensePlate = "ABC-123";

            // Add driver
            Console.WriteLine($"Adding driver {driver1Id}");
            _parkingLot.AddAuthorizedDriver(driver1Id);

            // Add car
            _entryInputs.LicensePlate = car1LicensePlate;
            _entryInputs.DriverId = driver1Id;
            Console.WriteLine($"Driving car {_entryInputs.LicensePlate} in");

            // Drive in
            SimulateCarDrivingThroughGate(_entryGate, _entryInputs);

            // Drive out
            _exitInputs.LicensePlate = car1LicensePlate;
            Console.WriteLine($"Driving car {_exitInputs.LicensePlate} out");

            // Drive out
            SimulateCarDrivingThroughGate(_exitGate, _exitInputs);
        }

        static void CarParkIsFullTest()
        {
            string driverId = "OverflowDriver";
            _parkingLot.AddAuthorizedDriver(driverId);

            // Fill up the lot
            for (int i = 0; i < 150; i++)
            {
                string plate = $"CAR-{i:D3}";
                _entryInputs = new GateInputs
                {
                    DriverId = driverId,
                    LicensePlate = plate,
                };
                SimulateCarDrivingThroughGate(_entryGate, _entryInputs);
            }

            // Try one more car (should fail — lot is full)
            string extraPlate = "CAR-999";
            _entryInputs = new GateInputs
            {
                DriverId = driverId,
                LicensePlate = extraPlate,
                InductiveSensor = true
            };

            Console.WriteLine($"Attempting to enter with {extraPlate} when lot is full");

            // Only run 3 cycles to observe idle state
            GateOutputs outputs;
            for (int i = 0; i < 3; i++)
            {
                outputs = SimulateCycle(_entryGate, _entryInputs);

                if (outputs.GreenLight)
                {
                    break;
                }

                Console.WriteLine($"{_entryGate}: Waiting for green light");
            }
        }

        static void DriverIsInvalidTest()
        {
            string unauthorizedDriver = "BadDriver";
            string plate = "BAD-001";

            _entryInputs = new GateInputs
            {
                DriverId = unauthorizedDriver,
                LicensePlate = plate,
                InductiveSensor = true
            };

            Console.WriteLine($"Attempting to enter with unauthorized driver {unauthorizedDriver}");
            // Only run 3 cycles to observe idle state
            GateOutputs outputs;
            for (int i = 0; i < 3; i++)
            {
                outputs = SimulateCycle(_entryGate, _entryInputs);

                if (outputs.GreenLight)
                {
                    break;
                }

                Console.WriteLine($"{_entryGate}: Waiting for green light");
            }
        }

        static void EmergencyStopTest()
        {
            string driverId = "SafeDriver";
            string plate = "EMG-001";
            _parkingLot.AddAuthorizedDriver(driverId);

            _entryInputs = new GateInputs
            {
                DriverId = driverId,
                LicensePlate = plate,
                InductiveSensor = true
            };

            Console.WriteLine($"Simulating emergency stop during entry for car {plate}");

            // Start first cycle to go into validation
            SimulateCycle(_entryGate, _entryInputs);

            // Trigger EStop before opening
            _entryInputs.EStop = true;
            SimulateCycle(_entryGate, _entryInputs);

            // Check that no gate movement happens
            _entryInputs.EStop = false;
            SimulateCycle(_entryGate, _entryInputs);

            // Let car proceed normally
            SimulateCarDrivingThroughGate(_entryGate, _entryInputs);
        }

        static void DoubleEntryTest()
        {
            string driverId = "RepeatDriver";
            string plate = "REPEAT-001";

            _parkingLot.AddAuthorizedDriver(driverId);

            // First entry — should succeed
            _entryInputs = new GateInputs
            {
                DriverId = driverId,
                LicensePlate = plate,
                GateFullyClosed = true
            };
            SimulateCarDrivingThroughGate(_entryGate, _entryInputs);

            // Second entry — should fail (already in lot)
            _entryInputs = new GateInputs
            {
                DriverId = driverId,
                LicensePlate = plate,
                InductiveSensor = true,
                GateFullyClosed = true
            };

            Console.WriteLine($"Attempting to enter car {plate} again (should be denied)");
            GateOutputs outputs;
            for (int i = 0; i < 3; i++)
            {
                outputs = SimulateCycle(_entryGate, _entryInputs);
                if (outputs.GreenLight)
                {
                    Console.WriteLine("ERROR: Duplicate entry allowed!");
                    break;
                }
            }
        }

        static void InvalidExitTest()
        {
            string plate = "UNKNOWN-999";

            _exitInputs = new GateInputs
            {
                LicensePlate = plate,
                InductiveSensor = true,
                GateFullyClosed = true
            };

            Console.WriteLine($"Attempting to exit unknown car {plate} (should be denied)");
            GateOutputs outputs;
            for (int i = 0; i < 3; i++)
            {
                outputs = SimulateCycle(_exitGate, _exitInputs);
                if (outputs.GreenLight)
                {
                    Console.WriteLine("ERROR: Invalid exit allowed!");
                    break;
                }
            }
        }

        static void SafetySensorBlocksCloseTest()
        {
            string driverId = "SafeCloseDriver";
            string plate = "SAFE-001";

            _parkingLot.AddAuthorizedDriver(driverId);

            _entryInputs = new GateInputs
            {
                DriverId = driverId,
                LicensePlate = plate,
                GateFullyClosed = true
            };

            Console.WriteLine($"Testing gate closure prevention due to safety sensor for car {plate}");

            // Begin entry
            SimulateCycle(_entryGate, _entryInputs);

            // Simulate open gate and green light
            _entryInputs.InductiveSensor = true;
            _entryInputs.SafetySensor = true;
            _entryInputs.GateFullyOpen = true;

            GateOutputs outputs = SimulateCycle(_entryGate, _entryInputs);

            if (outputs.CloseGate)
            {
                Console.WriteLine("ERROR: Gate attempted to close while safety sensor active!");
            }
            else
            {
                Console.WriteLine("Gate did not close while safety sensor active");
            }

            // Clear sensor, allow gate to close
            _entryInputs.InductiveSensor = false;
            _entryInputs.SafetySensor = false;
            outputs = SimulateCycle(_entryGate, _entryInputs);
        }

    }
}