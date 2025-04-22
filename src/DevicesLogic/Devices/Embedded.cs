using System.Text.RegularExpressions;

namespace task7;

/// <summary>
/// This class represents an embedded device and inherits from device
/// </summary>
public class Embedded : Device
{
    public string NetworkName { get; set; }
    private string _ipAddress;
    private bool _isConnected = false;

    public Embedded()
    {
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            Regex ipRegex = new Regex("^((25[0-5]|(2[0-4]|1\\d|[1-9]|)\\d)\\.?\\b){4}$");
            if (ipRegex.IsMatch(value))
            {
                _ipAddress = value;
            }

            throw new ArgumentException("Wrong IP address format.");
        }
    }
    
    /// <summary>
    /// This is the constructor and I switched isEnabled to isConnected
    /// </summary>
    /// <param name="id">The unique identifier </param>
    /// <param name="name">The name </param>
    /// <param name="ipAddress">The IP address </param>
    /// <param name="isConnected">Indicates whether the device is connected to the network.</param>
    /// <param name="networkName">The name of the network </param>
    public Embedded(string id, string name, string ipAddress, bool isConnected, string networkName) : base(id, name)
    {
        if (CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: E-1", id);
        }

        IpAddress = ipAddress;
        NetworkName = networkName;
        _isConnected = isConnected;
    }

    /// <summary>
    /// This method disconnects an embedded device and I added it instead of using the turn of
    /// method to adhere to the Liskov substitution principle and interface segregation principle
    /// </summary>
    public void Disconnect()
    {
        _isConnected = false;
    }

    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        return $"Embedded device {Name} ({Id}) is {enabledStatus} and has IP address {IpAddress}";
    }
    
    /// <summary>
    /// This method formats device data to be saved
    /// </summary>
    public override string saveDevice()
    {
        return $"{Id},{Name},{IsEnabled},{IpAddress},{NetworkName}";
    }

    
    /// <summary>
    /// This method connects an embedded device
    /// </summary>
    private void Connect()
    {
        if (NetworkName.Contains("MD Ltd."))
        {
            _isConnected = true;
        }
        else
        {
            throw new ConnectionException();
        }
    }
    
    private bool CheckId(string id) => id.Contains("E-");
}