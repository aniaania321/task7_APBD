namespace task7;

/// <summary>
/// Now the SW class inherits from turnDevices instead of Device in order to differentiate that it can be turned on and off
/// </summary>
public class Smartwatch : TurnDevices, IPowerNotify
{
    private int _batteryLevel;

    public int BatteryLevel
    {
        get => _batteryLevel;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("Invalid battery level value. Must be between 0 and 100.", nameof(value));
            }
            
            _batteryLevel = value;
            if (_batteryLevel < 20)
            {
                Notify();
            }
        }
    }

    public Smartwatch()
    {
    }

    public Smartwatch(string id, string name, bool isEnabled, int batteryLevel) : base(id, name, isEnabled)
    {
        if (CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: SW-1", id);
        }
        BatteryLevel = batteryLevel;
    }

    public void Notify()
    {
        Console.WriteLine($"Battery level is low. Current level is: {BatteryLevel}");
    }

    public override void TurnOn()
    {
        if (BatteryLevel < 11)
        {
            throw new EmptyBatteryException();
        }

        base.TurnOn();
        BatteryLevel -= 10;

        if (BatteryLevel < 20)
        {
            Notify();
        }
    }

    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        return $"Smartwatch {Name} ({Id}) is {enabledStatus} and has {BatteryLevel}%";
    }
    
    /// <summary>
    /// This method formats device data to be saved
    /// </summary>
    /// <returns>
    /// String representation of the device
    /// </returns>
    public override string saveDevice()
    {
        return $"{Id},{Name},{IsEnabled},{BatteryLevel}%";
    }
    
    private bool CheckId(string id) => id.Contains("E-");
}