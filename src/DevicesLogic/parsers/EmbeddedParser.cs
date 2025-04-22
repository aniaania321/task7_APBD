namespace task7;

/// <summary>
/// The class to parse the embedded device
/// </summary>
public class EmbeddedParser:DeviceParserInterface
{
    public bool whichDevice(string line)
    {
        if (line.StartsWith("ED-"))
            return true;
        else
        {
            return false;
        }
    }

    private const int MinimumRequiredElements = 4;

    private const int IndexPosition = 0;
    private const int DeviceNamePosition = 1;
    private const int EnabledStatusPosition = 2;
    
    /// <summary>
    /// This method does the same thing that was the resposinibility of the device parser previously
    /// </summary>
    /// <param name="line">
    /// The line of data to be parsed
    /// </param>
    /// <param name="lineNumber">
    /// Used for reporting errors
    /// </param>
    /// <returns>
    /// A new `Embedded` device
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Error when there is the wrong number of elements
    /// </exception>
    public Device parse(string line, int lineNumber)
    {
        const int IpAddressPosition = 3;
        const int NetworkNamePosition = 4;
        
        var infoSplits = line.Split(',');

        if (infoSplits.Length < MinimumRequiredElements + 1)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}", line);
        }
        
        if (bool.TryParse(infoSplits[EnabledStatusPosition], out bool _) is false)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for embedded device.", line);
        }

        return new Embedded(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], 
            infoSplits[IpAddressPosition],bool.Parse(infoSplits[EnabledStatusPosition]), 
            infoSplits[NetworkNamePosition]);
    }
}