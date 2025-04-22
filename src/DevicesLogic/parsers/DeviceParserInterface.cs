namespace task7;

/// <summary>
/// I created this interface so that when we add another device we can just add another
/// class to parse it and no additional code has to be modified.
/// This interface defines the behaviour of such new classes.
/// </summary>
public interface DeviceParserInterface
{
    /// <summary>
    /// This method checks the device type given in the line.
    /// </summary>
    /// <param name="line">
    /// Data representing a device to be checked
    /// </param>
    /// <returns>
    /// Boolean indicating whether the device type matches the device
    /// </returns>
    bool whichDevice(string line);

    /// <summary>
    /// This method parses the device and will be implemented in each class separately.
    /// </summary>
    /// <param name="line">
    /// Data representing a device to be parsed
    /// </param>
    /// <param name="lineNumber">
    /// The line number for error reporting if the line cannot be parsed
    /// </param>
    /// <returns>
    /// The Device that wa screated with the parsed lines
    /// </returns>
    Device parse(string line, int lineNumber);

}