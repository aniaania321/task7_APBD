namespace task7;

/// <summary>
/// Now the PC class inherits from turnDevices instead of Device in order to differentiate that it can be turned on and off
/// </summary>
public class PersonalComputer : TurnDevices
{
    public string? OperatingSystem { get; set; }

    public PersonalComputer()
    {
    }

    public PersonalComputer(string id, string name, bool isEnabled, string? operatingSystem) : base(id, name, isEnabled)
    {
        if (!CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: P-1", id);
        }
        
        OperatingSystem = operatingSystem;
    }

    public override void TurnOn()
    {
        if (OperatingSystem is null)
        {
            throw new EmptySystemException();
        }

        base.TurnOn();
    }

    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        string osStatus = OperatingSystem is null ? "has not OS" : $"has {OperatingSystem}";
        return $"PC {Name} ({Id}) is {enabledStatus} and {osStatus}";
    }
    
    /// <summary>
    /// This method formats device data to be saved
    /// </summary>
    /// <returns>
    /// String representation of the device
    /// </returns>
    public override string saveDevice()
    {
        return $"{Id},{Name},{IsEnabled},{OperatingSystem ?? "null"}";
    }

    
    private bool CheckId(string id) => id.Contains("P-");
}