namespace task7;

/// <summary>
/// I created this interface so that the device manager doesn't directly
/// depend on the file service and therefore the code adheres to the dependency inversion principle
/// </summary>
public interface DataInterface
{
    /// <summary>
    /// This method is used to get all devices from the file
    /// </summary>
    /// <returns>
    /// A string array containing the devices
    /// </returns>
    public string[] GetDevices();
    
    /// <summary>
    /// This method is used to save the modified devices to the file
    /// </summary>
    /// param name="devices">
    /// The list of devices to be saved to the file
    /// </param>
    public void SaveDevices(List<Device> devices);

}