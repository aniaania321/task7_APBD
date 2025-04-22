namespace DevicesObjects;

public class PersonalComputerDTO
{
    public int Id { get; set; }
    public string OperationSystem { get; set; }
    public string DeviceId { get; set; }

    public DeviceDTO Device { get; set; }
}
