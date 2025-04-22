namespace task7;
/// <summary>
/// The class to parse the PC
/// </summary>
public class PCParser:DeviceParserInterface
{
    public bool whichDevice(string line)
    {
        if (line.StartsWith("P-"))
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
    
    private const int _SystemPosition = 3;

    /// <summary>
    /// This method does the same thing that was the resposinibility of the device parser previously
    /// </summary>
    /// <param name="line">
    /// Device data
    /// </param>
    /// <param name="lineNumber">
    /// Used for error
    /// </param>
    /// <returns>
    /// A PC object returned that was created
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If a line cannot be parsed
    /// </exception>
    public Device parse(string line, int lineNumber)
    {

    var infoSplits = line.Split(',');

        if (infoSplits.Length < MinimumRequiredElements)
    {
        throw new ArgumentException($"Corrupted line {lineNumber}", line);
    }
        
    if (bool.TryParse(infoSplits[EnabledStatusPosition], out bool _) is false)
    {
        throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for computer.", line);
    }
        
    return new PersonalComputer(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], 
    bool.Parse(infoSplits[EnabledStatusPosition]), infoSplits[_SystemPosition]);
    }
    
}