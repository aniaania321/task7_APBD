namespace DevicesObjects;

public class EmbeddedDTO
{
    public int Id { get; set; }
    public string IpAddress { get; set; }
    public string NetworkName { get; set; }
    public string DeviceId { get; set; }
    public DeviceDTO Device { get; set; }
}
