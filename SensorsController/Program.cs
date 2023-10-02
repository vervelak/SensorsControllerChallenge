using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


public class Sensor
{
    public int Position { get; }
    public string Type { get; }
    public int SensorValue { get; private set; }
    public bool IsRecording { get; private set; }
    private int offset;  // Offset for tilted sensors

    public Sensor(int position, string type, int offset)
    {
        Position = position;
        Type = type;
        SensorValue = 0;
        IsRecording = false;
        this.offset = offset;
    }

    public void StartRecording()
    {
        IsRecording = true;
        int adjustedValue = SensorValue;
        SensorValue = 5;
        if (Type == "x-axis" && offset > 0)
            adjustedValue -= offset;
        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Sensor {Position} {Type} start {adjustedValue}");
    }

    public void StopRecording()
    {
        int adjustedValue = SensorValue;
        if (Type == "x-axis" && offset > 0)
            adjustedValue -= offset;
        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Sensor {Position} {Type} stop {adjustedValue}");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: controller_program.exe <control pattern>");
            return;
        }

        string controlPattern = args[0];
        List<Sensor> sensors = InitializeSensors(20); // Initialize 20 sensors

        if (controlPattern.ToLower() == "sequence")
        {
            // Control the sensors in sequential order
            for (int i = 0; i < sensors.Count; i++)
            {
                sensors[i].StartRecording();
                await Task.Delay(1000); // Simulate 1 second delay for sensor recording
                sensors[i].StopRecording();
            }
        }
        else if (controlPattern.ToLower() == "type")
        {
            // Start recording for all x-axis sensors asynchronously
            var tasks = new List<Task>();
            foreach (var sensor in sensors)
            {
                if (sensor.Type == "x-axis")
                {
                    tasks.Add(Task.Run(() =>
                    {
                        sensor.StartRecording();
                        Thread.Sleep(1000);
                        sensor.StopRecording();
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }
        else
        {
            Console.WriteLine("Invalid control pattern. Supported patterns: sequence, type");
        }
    }

    static List<Sensor> InitializeSensors(int numSensors)
    {
        List<Sensor> sensors = new List<Sensor>();
        string[] sensorTypes = { "x-axis", "y-axis", "z-axis" };
        int sensorTypeIndex = 0;
        int numTiltedSensors = 5;
        int tiltOffset = 3;

        for (int i = 1; i <= numSensors; i++)
        {
            string sensorType = sensorTypes[sensorTypeIndex];
            int offset = (sensorType == "x-axis" && i > numSensors - numTiltedSensors) ? tiltOffset : 0;
            Sensor sensor = new Sensor(i, sensorType, offset);
            sensors.Add(sensor);

            sensorTypeIndex = (sensorTypeIndex + 1) % 3; // Rotate through the sensor types
        }

        return sensors;
    }
}
