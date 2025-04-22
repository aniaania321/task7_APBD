namespace task7;

/// <summary>
/// I created this interface so that the device manager doesn't directly
/// depend on the device parser and therefore the code adheres to the dependency inversion principle
/// </summary>
public interface ParserIntreface
{
    /// <summary>
    /// This is the method used to parse a number of devices
    /// </summary>
    /// <param name="lines">
    /// Array with device information
    /// </param>
    /// <returns>
    /// List of `Device` objects parsed 
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Error if the line can't be parsed
    /// </exception>
    public List<Device> ParseDevices(string[] lines);
}