namespace task7;

public class DeviceParser: ParserIntreface
{
    private List<DeviceParserInterface> _parsers;

    public DeviceParser()
    {
        _parsers = new List<DeviceParserInterface>
        {
            new PCParser(),
            new SmartwatchParser(),
            new EmbeddedParser()
        };
    }
    /// <summary>
    /// Instead of checking manually what type the device is, I use the deviceParserInterface I created.
    /// This makes sure the code adheres to the open-closed principle
    /// </summary>
    /// <param name="lines">
    /// Array of strings with lines to parse
    /// </param>
    /// <returns>
    /// A list of devices parsed
    /// </returns>
    public List<Device> ParseDevices(string[] lines)
    {
        List<Device> devices = new List<Device>();

        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                Device parsedDevice = null;

                foreach (var parser in _parsers)
                {
                    if (parser.whichDevice(lines[i]))
                    {
                        parsedDevice = parser.parse(lines[i], i);
                        break;
                    }
                }

                if (parsedDevice == null)
                {
                    throw new ArgumentException($"Line {i} was failed to parse.");
                }

                devices.Add(parsedDevice);
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine(argEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return devices;
    }
}