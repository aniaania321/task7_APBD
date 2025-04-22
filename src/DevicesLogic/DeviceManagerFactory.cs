namespace task7;

/// <summary>
/// I created the DeviceManagerFactory class to be able to create
/// an instance of DeviceManager with usage of factory pattern.
/// </summary>
public class DeviceManagerFactory
{
    public static DeviceManager CreateDeviceManager(DataInterface fileService, ParserIntreface deviceParser)
    {
        return new DeviceManager(fileService, deviceParser);
    }
}