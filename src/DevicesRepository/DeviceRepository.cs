using System.Data;
using DevicesObjects;

namespace DevicesRepository;

using Microsoft.Data.SqlClient;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<DeviceDTO> GetAllDevices()
    {
        var containers = new List<DeviceDTO>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT * FROM Device", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            containers.Add(new DeviceDTO
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsEnabled = reader.GetBoolean(2)
            });
        }

        return containers;
    }

    public object? GetDeviceById(string deviceId, string deviceType)
    {
        string query = deviceType switch
        {
            "Embedded" => @"SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled,e.Id AS EmbeddedId, e.IpAddress, e.NetworkName FROM Embedded e JOIN Device d ON e.DeviceId = d.Id WHERE d.Id = @deviceId",
            "PersonalComputer" => @"SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled, pc.Id AS PcId, pc.OperationSystem FROM PersonalComputer pc JOIN Device d ON pc.DeviceId = d.Id WHERE d.Id = @deviceId",
            "Smartwatch" => @"SELECT d.Id AS DeviceId, d.Name AS DeviceName, d.IsEnabled, s.Id AS SmartwatchId, s.BatteryPercentage FROM Smartwatch s JOIN Device d ON s.DeviceId = d.Id WHERE d.Id = @deviceId",
            _ => null
        };

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@deviceId", deviceId);
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;

        var device = new DeviceDTO
        {
            Id = reader["DeviceId"].ToString(),
            Name = reader["DeviceName"].ToString(),
            IsEnabled = Convert.ToBoolean(reader["IsEnabled"])
        };

        return deviceType switch
        {
            "Embedded" => new EmbeddedDTO
            {
                Id = Convert.ToInt32(reader["EmbeddedId"]),
                IpAddress = reader["IpAddress"].ToString(),
                NetworkName = reader["NetworkName"]?.ToString(),
                Device = device
            },
            "PersonalComputer" => new PersonalComputerDTO
            {
                Id = Convert.ToInt32(reader["PcId"]),
                OperationSystem = reader["OperationSystem"]?.ToString(),
                Device = device
            },
            "Smartwatch" => new SmartwatchDTO
            {
                Id = Convert.ToInt32(reader["SmartwatchId"]),
                BatteryPercentage = Convert.ToInt32(reader["BatteryPercentage"]),
                Device = device
            },
            _ => null
        };
    }

    public void InsertEmbedded(EmbeddedDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var cmd = new SqlCommand("AddEmbedded", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DeviceId", dto.DeviceId);
            cmd.Parameters.AddWithValue("@Name", dto.Device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            cmd.Parameters.AddWithValue("@IpAddress", dto.IpAddress);
            cmd.Parameters.AddWithValue("@NetworkName", dto.NetworkName);

            cmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"[Error] Failed to insert embedded device: {ex.Message}");
            throw;
        }
    }


    public void InsertPC(PersonalComputerDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var cmd = new SqlCommand("AddPersonalComputer", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DeviceId", dto.DeviceId);
            cmd.Parameters.AddWithValue("@Name", dto.Device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            cmd.Parameters.AddWithValue("@OperationSystem", dto.OperationSystem);

            cmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"[Error] Failed to insert personal computer: {ex.Message}");
            throw;
        }
    }

    public void InsertSmartwatch(SmartwatchDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var cmd = new SqlCommand("AddSmartwatch", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DeviceId", dto.DeviceId);
            cmd.Parameters.AddWithValue("@Name", dto.Device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            cmd.Parameters.AddWithValue("@BatteryPercentage", dto.BatteryPercentage);

            cmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"[Error] Failed to insert smartwatch: {ex.Message}");
            throw;
        }
    }
    
    public void UpdateEmbedded(string deviceId, EmbeddedDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var updateDeviceCmd = new SqlCommand(@"UPDATE Device SET IsEnabled = @IsEnabled WHERE Id = @DeviceId AND RowVersion = @RowVersion", connection, transaction);

            updateDeviceCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateDeviceCmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            updateDeviceCmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value=dto.Device.RowVersion;

            int affected = updateDeviceCmd.ExecuteNonQuery();
            if (affected == 0)
                throw new DBConcurrencyException("Device has been modified by another user.");

            using var updateCmd = new SqlCommand(@"UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId", connection, transaction);

            updateCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateCmd.Parameters.AddWithValue("@IpAddress", dto.IpAddress);
            updateCmd.Parameters.AddWithValue("@NetworkName", dto.NetworkName ?? (object)DBNull.Value);
            updateCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdatePC(string deviceId, PersonalComputerDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var updateDeviceCmd = new SqlCommand(@" UPDATE Device SET IsEnabled = @IsEnabled WHERE Id = @DeviceId AND RowVersion = @RowVersion", connection, transaction);

            updateDeviceCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateDeviceCmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            updateDeviceCmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value=dto.Device.RowVersion;

            int affected = updateDeviceCmd.ExecuteNonQuery();
            if (affected == 0)
                throw new DBConcurrencyException("Device has been modified by another user.");

            using var updateCmd = new SqlCommand(@"UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId", connection, transaction);

            updateCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateCmd.Parameters.AddWithValue("@OperationSystem", dto.OperationSystem);
            updateCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public void UpdateSmartwatch(string deviceId, SmartwatchDTO dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var updateDeviceCmd = new SqlCommand(@"UPDATE Device SET IsEnabled = @IsEnabled WHERE Id = @DeviceId AND RowVersion = @RowVersion", connection, transaction);

            updateDeviceCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateDeviceCmd.Parameters.AddWithValue("@IsEnabled", dto.Device.IsEnabled);
            updateDeviceCmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value=dto.Device.RowVersion;

            int affected = updateDeviceCmd.ExecuteNonQuery();
            if (affected == 0)
                throw new DBConcurrencyException("Device has been modified by another user.");

            using var updateSmartwatchCmd = new SqlCommand(@"UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId", connection, transaction);

            updateSmartwatchCmd.Parameters.AddWithValue("@DeviceId", deviceId);
            updateSmartwatchCmd.Parameters.AddWithValue("@BatteryPercentage", dto.BatteryPercentage);
            updateSmartwatchCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }


    public bool DeleteDevice(string deviceId, string deviceType)
    {
        string query = deviceType switch
        {
            "Embedded" => @"DELETE FROM Embedded WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            "PersonalComputer" => @"DELETE FROM PersonalComputer WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            "Smartwatch" => @"DELETE FROM Smartwatch WHERE DeviceId = @deviceId; DELETE FROM Device WHERE Id = @deviceId;",
            _ => throw new Exception("Invalid device type")
        };

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@deviceId", deviceId);
            int affected = cmd.ExecuteNonQuery();

            if (affected > 0)
            {
                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
