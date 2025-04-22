namespace task7;

/// <summary>
/// I implemented an TurnDevices interface that inherits from Device class in order to differentiate between
/// devices that can be turned on and off and ones that cannot (such as embedded device).
/// This makes sure the Liskov substitution principle and the interface segregation principle are followed
/// </summary>
public abstract class TurnDevices:Device
{
    public bool IsEnabled { get; set; }

    protected TurnDevices(string id, string name, bool isEnabled) : base(id, name)
    {
        IsEnabled = isEnabled;
    }

    protected TurnDevices()
    {
    }

    /// <summary>
    /// Methods to turn devices on and off
    /// </summary>
    public virtual void TurnOn()
    {
        IsEnabled = true;
    }

    public virtual void TurnOff()
    {
        IsEnabled = false;
    }
    
    /// <summary>
    /// This method formats device data to be saved
    /// </summary>
    /// <returns>
    /// String representation of the device
    /// </returns>
    public override string saveDevice()
    {
        return $"{Id},{Name},{IsEnabled}";
    }
}