using DevicesObjects;


namespace task7;

public interface IDeviceService
{
    IEnumerable<DeviceDTO> GetAllDevices();
    object? GetDeviceById(string id, string deviceType);
    void CreateDevice(object deviceDto, string deviceType, string deviceName,bool isEnabled);

    bool DeleteDeviceById(string deviceId, string deviceType);
    void UpdateDevice(object deviceDto, string deviceType);
}