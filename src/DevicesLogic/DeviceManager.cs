namespace task7;

public class DeviceManager
{
    private readonly ParserIntreface _deviceParser;
    private readonly DataInterface _fileService;
    private const int MaxCapacity = 15;
    private static List<Device> _devices = new(capacity: MaxCapacity);

    public List<Device> GetAllDevices()
    {
        return _devices;
    }   

    public DeviceManager(DataInterface fileService, ParserIntreface deviceParser)
    {
        _fileService = fileService;
        string[] lines=fileService.GetDevices(); 
        _deviceParser = deviceParser;
        _devices = _deviceParser.ParseDevices(lines);
    }

    public void AddDevice(Device newDevice)
    {
        foreach (var storedDevice in _devices)
        {
            if (storedDevice.Id.Equals(newDevice.Id))
            {
                throw new ArgumentException($"Device with ID {storedDevice.Id} is already stored.", nameof(newDevice));
            }
        }

        if (_devices.Count >= MaxCapacity)
        {
            throw new Exception("Device storage is full.");
        }
        
        _devices.Add(newDevice);
        _fileService.SaveDevices(_devices);
    }
    
    /// <summary>
    /// I decided to delete the if statements in this method and make it
    /// universal for all devices making it adhere to the open-closed principle
    /// </summary>
    /// <param name="editDevice">
    /// The device with the new details
    /// </param>
    /// <exception cref="ArgumentException">
    /// Error if there is no device with the id we want
    /// </exception>
    public void EditDevice(Device editDevice)
    {
        var targetDeviceIndex = -1;
        for (var index = 0; index < _devices.Count; index++)
        {
            var storedDevice = _devices[index];
            if (storedDevice.Id.Equals(editDevice.Id))
            {
                targetDeviceIndex = index;
                break;
            }
        }

        if (targetDeviceIndex == -1)
        {
            throw new ArgumentException($"Device with ID {editDevice.Id} is not stored.", nameof(editDevice));
        }

        
        _devices[targetDeviceIndex] = editDevice;
        _fileService.SaveDevices(_devices);
        
    }
    public void RemoveDeviceById(string deviceId)
    {
        Device? targetDevice = null;
        foreach (var storedDevice in _devices)
        {
            if (storedDevice.Id.Equals(deviceId))
            {
                targetDevice = storedDevice;
                break;
            }
        }

        if (targetDevice == null)
        {
            throw new ArgumentException($"Device with ID {deviceId} is not stored.", nameof(deviceId));
        }
        
        _devices.Remove(targetDevice);
        _fileService.SaveDevices(_devices);
    }

    public void TurnOnDevice(string id)
    {
        var storedDevice = GetDeviceById(id);

        if (storedDevice is TurnDevices controllableDevice)
        {
            controllableDevice.TurnOn();
        }
        else
        {
            throw new InvalidOperationException($"Device {id} cannot be turned on.");
        }
        _fileService.SaveDevices(_devices);
    }

    public void TurnOffDevice(string id)
    {
        var storedDevice = GetDeviceById(id);

        if (storedDevice is TurnDevices controllableDevice)
        {
            controllableDevice.TurnOff();
        }
        else
        {
            throw new InvalidOperationException($"Device {id} cannot be turned off.");
        }
        _fileService.SaveDevices(_devices);
    }


    public Device? GetDeviceById(string id)
    {
        foreach (var storedDevice in _devices)
        {
            if (storedDevice.Id.Equals(id))
            {
                return storedDevice;
            }
        }

        return null;
    }

    public void ShowAllDevices()
    {
        foreach (var storedDevices in _devices)
        {
            Console.WriteLine(storedDevices.ToString());
        }
    }
    
}