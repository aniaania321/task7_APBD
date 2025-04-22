namespace task7;

/// <summary>
/// This abstract class defines different devices
/// </summary>
public abstract class Device
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    public Device()
    {
    }

    public Device(string id, string name)
    {
        Id = id;
        Name = name;
    }
    
    /// <summary>
    /// This method was added so that we don't have to differentiate for different devices in the FileService class therefore making them independent (Open-closed principle) 
    /// It is implemented by derived classes and formats device data
    /// </summary>
    /// <returns>A string representation of the device's data.</returns>
    public abstract string saveDevice();
    
}
