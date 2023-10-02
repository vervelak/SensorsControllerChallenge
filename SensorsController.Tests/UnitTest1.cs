using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SensorsController.Tests
{
[TestFixture]
public class SensorTests
{
    [Test]
    public void StartRecording_UpdatesIsRecordingAndSensorValue()
    {
        // Arrange
        Sensor sensor = new Sensor(1, "x-axis", 2);

        // Act
        sensor.StartRecording();

        // Assert
        Assert.IsTrue(sensor.IsRecording);
        Assert.AreEqual(5, sensor.SensorValue);
    }

    [Test]
    public void StopRecording_DoesNotUpdateSensorValue()
    {
        // Arrange
        Sensor sensor = new Sensor(1, "x-axis", 2);

        // Act
        sensor.StopRecording();

        // Assert
        Assert.IsFalse(sensor.IsRecording);
        Assert.AreEqual(0, sensor.SensorValue);
    }
}

[TestFixture]
public class ProgramTests
{
    [Test]
    public async Task Main_InvalidControlPattern_PrintsErrorMessage()
    {
        // Arrange
        string[] args = { "invalid-pattern" };
        var consoleOutput = new ConsoleOutput();

        // Act
        await Program.Main(args);

        // Assert
        Assert.IsTrue(consoleOutput.GetOutput().Contains("Invalid control pattern."));
    }

    [Test]
    public async Task Main_SingleSensorSequenceControl_Succeeds()
    {
        // Arrange
        string[] args = { "sequence" };
        var consoleOutput = new ConsoleOutput();

        // Act
        await Program.Main(args);

        // Assert
        Assert.IsTrue(consoleOutput.GetOutput().Contains("Sensor 1 x-axis start"));
        Assert.IsTrue(consoleOutput.GetOutput().Contains("Sensor 1 x-axis stop"));
    }

    [Test]
    public async Task Main_TypeControl_StartsAndStopsAllXAxisSensors()
    {
        // Arrange
        string[] args = { "type" };
        var consoleOutput = new ConsoleOutput();

        // Act
        await Program.Main(args);

        // Assert
        Regex regex = new Regex(@"Sensor \d+ x-axis start");
        Assert.IsTrue(regex.Match(consoleOutput.GetOutput()).Success);
        Assert.IsTrue(regex.Match(consoleOutput.GetOutput()).Success);
        // Add more assertions based on the expected behavior
    }
}

// Helper class to capture console output
public class ConsoleOutput : IDisposable
{
    private readonly System.IO.StringWriter stringWriter;
    private readonly System.IO.TextWriter originalOutput;

    public ConsoleOutput()
    {
        stringWriter = new System.IO.StringWriter();
        originalOutput = Console.Out;
        Console.SetOut(stringWriter);
    }

    public string GetOutput()
    {
        return stringWriter.ToString();
    }

    public void Dispose()
    {
        Console.SetOut(originalOutput);
        stringWriter.Dispose();
    }
}
}
