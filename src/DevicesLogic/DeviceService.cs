using System.Text.RegularExpressions;
using DevicesObjects;

namespace task7;

using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DeviceService : IDeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        List<DeviceDTO> containers = new List<DeviceDTO>();
        string queryString = "SELECT * FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new DeviceDTO
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsEnabled = reader.GetBoolean(2)
                        };
                        containers.Add(container);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return containers;
    }


    public object? GetDeviceById(string deviceId, string deviceType)
{
    string query = deviceType switch
    {
        "Embedded" => @"SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled,e.Id AS EmbeddedId, e.IpAddress, e.NetworkName FROM Embedded e JOIN Device d ON e.DeviceId = d.Id WHERE d.Id = @deviceId",
        "PersonalComputer" => @"SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled, pc.Id AS PcId, pc.OperationSystem FROM PersonalComputer pc JOIN Device d ON pc.DeviceId = d.Id WHERE d.Id = @deviceId",
        "Smartwatch" => @" SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled, s.Id AS SmartwatchId, s.BatteryPercentage FROM Smartwatch s JOIN Device d ON s.DeviceId = d.Id WHERE d.Id = @deviceId",
        _ => null
    };

    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@deviceId", deviceId);

        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {

                var device = new DeviceDTO
                {
                    Id = reader["DeviceId"].ToString(),
                    Name = reader["DeviceName"].ToString(),
                    IsEnabled = Convert.ToBoolean(reader["IsEnabled"])
                };
                if (deviceType == "Embedded")
                {
                    return new EmbeddedDTO
                    {
                        Id = Convert.ToInt32(reader["EmbeddedId"]),
                        IpAddress = reader["IpAddress"].ToString(),
                        NetworkName = reader["NetworkName"]?.ToString(),
                        Device = device
                    };
                }
                else if (deviceType == "PersonalComputer")
                {
                    return new PersonalComputerDTO
                    {
                        Id = Convert.ToInt32(reader["PcId"]),
                        OperationSystem = reader["OperationSystem"]?.ToString(),
                        Device = device
                    };
                }
                else if (deviceType == "Smartwatch")
                {
                    return new SmartwatchDTO
                    {
                        Id = Convert.ToInt32(reader["SmartwatchId"]),
                        BatteryPercentage = Convert.ToInt32(reader["BatteryPercentage"]),
                        Device = device
                    };
                }
            }
        }
    }

    return null;
}
    //Here i get the int id for the specific device because my database dosen't do it on its own
    public int GetId(SqlConnection connection, string tableName)
    {
        using var cmd = new SqlCommand($"SELECT ISNULL(MAX(Id), 0) + 1 FROM {tableName}", connection);
        return (int)cmd.ExecuteScalar();
    }
    
    public void CreateDevice(object deviceDto, string deviceType, string deviceName, bool isEnabled)
{
    using var connection = new SqlConnection(_connectionString);
    connection.Open();

    var newDeviceId = Guid.NewGuid().ToString();
    int newSpecificId = 0;

    using (var cmd = new SqlCommand("INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)", connection))
    {
        cmd.Parameters.AddWithValue("@Id", newDeviceId);
        cmd.Parameters.AddWithValue("@Name", deviceName);
        cmd.Parameters.AddWithValue("@IsEnabled", isEnabled ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    if (deviceType == "Embedded")
    {
        var embeddedDto = deviceDto as EmbeddedDTO;

        var ipAddressPattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        if (!Regex.IsMatch(embeddedDto.IpAddress, ipAddressPattern))
        {
            throw new ArgumentException("Invalid IP");
        }

        newSpecificId = GetId(connection, "Embedded");

        using (var cmd = new SqlCommand("INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES (@Id, @IpAddress, @NetworkName, @DeviceId)", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@IpAddress", embeddedDto.IpAddress);
            cmd.Parameters.AddWithValue("@NetworkName", embeddedDto.NetworkName);
            cmd.Parameters.AddWithValue("@DeviceId", newDeviceId);
            cmd.ExecuteNonQuery();
        }
    }
    else if (deviceType == "PersonalComputer")
    {
        var pcDto = deviceDto as PersonalComputerDTO;

        if (string.IsNullOrEmpty(pcDto.OperationSystem))
        {
            throw new EmptySystemException();
        }

        newSpecificId = GetId(connection, "PersonalComputer");

        using (var cmd = new SqlCommand("INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId) VALUES (@Id, @OperationSystem, @DeviceId)", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@OperationSystem", pcDto.OperationSystem);
            cmd.Parameters.AddWithValue("@DeviceId", newDeviceId);
            cmd.ExecuteNonQuery();
        }
    }
    else if (deviceType == "Smartwatch")
    {
        var watchDto = deviceDto as SmartwatchDTO;

        if (watchDto.BatteryPercentage < 0 || watchDto.BatteryPercentage > 100)
        {
            throw new ArgumentException("Invalid battery.");
        }

        if (watchDto.BatteryPercentage < 11)
        {
            throw new EmptyBatteryException();
        }

        newSpecificId = GetId(connection, "Smartwatch");

        using (var cmd = new SqlCommand("INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId)VALUES (@Id, @BatteryPercentage, @DeviceId)", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@BatteryPercentage", watchDto.BatteryPercentage);
            cmd.Parameters.AddWithValue("@DeviceId", newDeviceId);
            cmd.ExecuteNonQuery();
        }
    }
    else
    {
        throw new Exception("Invalid device type");
    }
}
    public void UpdateDevice(object deviceDto, string deviceType)
{
    using var connection = new SqlConnection(_connectionString);
    connection.Open();

    int newSpecificId = 0;

    if (deviceType == "Embedded")
    {
        var embeddedDto = deviceDto as EmbeddedDTO;

        var ipAddressPattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        if (!Regex.IsMatch(embeddedDto.IpAddress, ipAddressPattern))
        {
            throw new ArgumentException("Invalid IP Address format.");
        }

        newSpecificId = GetDeviceId(connection, "Embedded", embeddedDto.DeviceId);

        using (var cmd = new SqlCommand("UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE Id = @Id", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@IpAddress", embeddedDto.IpAddress);
            cmd.Parameters.AddWithValue("@NetworkName", embeddedDto.NetworkName);
            cmd.ExecuteNonQuery();
        }
    }
    else if (deviceType == "PersonalComputer")
    {
        var pcDto = deviceDto as PersonalComputerDTO;

        if (string.IsNullOrEmpty(pcDto.OperationSystem))
        {
            throw new EmptySystemException();
        }

        newSpecificId = GetDeviceId(connection, "PersonalComputer", pcDto.DeviceId);

        using (var cmd = new SqlCommand("UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE Id = @Id", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@OperationSystem", pcDto.OperationSystem);
            cmd.ExecuteNonQuery();
        }
    }
    else if (deviceType == "Smartwatch")
    {
        var watchDto = deviceDto as SmartwatchDTO;

        if (watchDto.BatteryPercentage < 0 || watchDto.BatteryPercentage > 100)
        {
            throw new ArgumentException("Invalid battery.");
        }

        if (watchDto.BatteryPercentage < 11)
        {
            throw new EmptyBatteryException();
        }

        newSpecificId = GetDeviceId(connection, "Smartwatch", watchDto.DeviceId);

        using (var cmd = new SqlCommand("UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE Id = @Id", connection))
        {
            cmd.Parameters.AddWithValue("@Id", newSpecificId);
            cmd.Parameters.AddWithValue("@BatteryPercentage", watchDto.BatteryPercentage);
            cmd.ExecuteNonQuery();
        }
    }
    else
    {
        throw new Exception("Invalid device type");
    }
}

    //Getting device's specific id from generic DeviceId
    public int GetDeviceId(SqlConnection connection, string deviceType, string deviceId)
{
    using var cmd = new SqlCommand($"SELECT Id FROM {deviceType} WHERE DeviceId = @DeviceId", connection);
    cmd.Parameters.AddWithValue("@DeviceId", deviceId);

    using var reader = cmd.ExecuteReader();
    if (reader.Read())
    {
        return reader.GetInt32(0);
    }
    else
    {
        throw new Exception("Device not found");
    }
}
    
    public bool DeleteDeviceById(string deviceId, string deviceType)
    {
        string queryString = deviceType switch
        {
            "Embedded" => @"DELETE FROM Embedded WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            "PersonalComputer" => @" DELETE FROM PersonalComputer WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            "Smartwatch" => @"DELETE FROM Smartwatch WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            _ => null
        };

        if (queryString == null)
        {
            throw new Exception("Invalid device type");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@deviceId", deviceId);

            connection.Open();

            int affectedRows = command.ExecuteNonQuery();

            return affectedRows > 0;
        }
    }

}



