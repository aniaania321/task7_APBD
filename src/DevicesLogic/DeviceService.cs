using System.Text.RegularExpressions;
using DevicesObjects;
using DevicesRepository;

namespace task7;

using System.Collections.Generic;

public class DeviceService : IDeviceService
{
    private string _connectionString;
    private readonly DeviceRepository _repository;

    public DeviceService(string connectionString)
    {
        _repository = new DeviceRepository(connectionString);
    }

    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        List<DeviceDTO> containers = _repository.GetAllDevices();

        return containers;
    }

    public object? GetDeviceById(string deviceId, string deviceType)
    {
        return _repository.GetDeviceById(deviceId, deviceType);

    }

    public void CreateDevice(object deviceDto, string deviceType, string deviceName, bool isEnabled)
{
    var device = new DeviceDTO
    {
        Id = Guid.NewGuid().ToString(),
        Name = deviceName,
        IsEnabled = isEnabled
    };

    switch (deviceType)
    {
        case "Embedded":
            var embeddedDto = deviceDto as EmbeddedDTO
                ?? throw new InvalidCastException("Invalid DTO type for Embedded");

            if (!Regex.IsMatch(embeddedDto.IpAddress ?? "", @"^(\d{1,3}\.){3}\d{1,3}$"))
                throw new ArgumentException("Invalid IP address format");

            if (string.IsNullOrWhiteSpace(embeddedDto.NetworkName))
                throw new ArgumentException("Network name cannot be empty");

            if (!embeddedDto.NetworkName.Contains("MD Ltd."))
                throw new ConnectionException();

            embeddedDto.Device = device;
            embeddedDto.DeviceId = device.Id;
            _repository.InsertEmbedded(embeddedDto);
            break;

        case "PersonalComputer":
            var pcDto = deviceDto as PersonalComputerDTO
                ?? throw new InvalidCastException("Invalid DTO type for PersonalComputer");

            if (string.IsNullOrWhiteSpace(pcDto.OperationSystem))
                throw new EmptySystemException();

            pcDto.Device = device;
            pcDto.DeviceId = device.Id;
            _repository.InsertPC(pcDto);
            break;

        case "Smartwatch":
            var watchDto = deviceDto as SmartwatchDTO
                ?? throw new InvalidCastException("Invalid DTO type for Smartwatch");

            if (watchDto.BatteryPercentage < 0 || watchDto.BatteryPercentage > 100)
                throw new ArgumentOutOfRangeException("Battery percentage must be between 0 and 100");

            if (isEnabled)
            {
                if (watchDto.BatteryPercentage < 11)
                    throw new EmptyBatteryException();

                watchDto.BatteryPercentage -= 10;
            }

            watchDto.Device = device;
            watchDto.DeviceId = device.Id;
            _repository.InsertSmartwatch(watchDto);
            break;

        default:
            throw new Exception("Invalid device type");
    }
}

    public void UpdateDevice(string deviceId, object deviceDto, string deviceType)
    {
        switch (deviceType)
        {
            case "Embedded":
                _repository.UpdateEmbedded(deviceId, (EmbeddedDTO)deviceDto);
                break;
            case "PersonalComputer":
                _repository.UpdatePC(deviceId, (PersonalComputerDTO)deviceDto);
                break;
            case "Smartwatch":
                _repository.UpdateSmartwatch(deviceId, (SmartwatchDTO)deviceDto);
                break;
            default:
                throw new Exception("Unsupported device type");
        }
    }


    public bool DeleteDeviceById(string deviceId, string deviceType)
    {
        return _repository.DeleteDevice(deviceId, deviceType);

    }
}



