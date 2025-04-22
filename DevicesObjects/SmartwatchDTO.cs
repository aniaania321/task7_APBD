namespace DevicesObjects;

public class SmartwatchDTO
{
    public int Id { get; set; }
    public int BatteryPercentage { get; set; }
    public string DeviceId { get; set; }

    public DeviceDTO Device { get; set; }
}
