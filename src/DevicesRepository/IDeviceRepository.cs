using DevicesObjects;

namespace DevicesRepository;

public interface IDeviceRepository
{
    public List<DeviceDTO> GetAllDevices();
    public object? GetDeviceById(string deviceId, string deviceType);
    public void InsertEmbedded(EmbeddedDTO dto);
    public void InsertPC(PersonalComputerDTO dto);
    public void InsertSmartwatch(SmartwatchDTO dto);
    public void UpdateEmbedded(string id,EmbeddedDTO dto);
    public void UpdatePC(string id,PersonalComputerDTO dto);
    public void UpdateSmartwatch(string id,SmartwatchDTO dto);
    public bool DeleteDevice(string deviceId, string deviceType);

}