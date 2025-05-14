using System.Text.Json;
using DevicesObjects;
using task7;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddTransient<IDeviceService, DeviceService>(_ => new DeviceService(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/api/devices", (IDeviceService deviceService) =>
{
    try
    {
        var devices = deviceService.GetAllDevices();
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/devices/{id}/{deviceType}", (IDeviceService deviceService, string id, string deviceType) =>
{
    try
    {
        var device = deviceService.GetDeviceById(id, deviceType);
        if (device == null)
        {
            return Results.NotFound("Device not found.");
        }

        return Results.Ok(device);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
app.MapPost("/api/devices", async (HttpRequest request, IDeviceService deviceService) =>
{
    try
    {
        using var reader = new StreamReader(request.Body);
        var content = await reader.ReadToEndAsync();

        var temp = JsonSerializer.Deserialize<JsonElement>(content);
        var deviceType = temp.GetProperty("deviceType").GetString();
        var deviceName = temp.GetProperty("deviceName").GetString();
        var isEnabled = temp.GetProperty("isEnabled").GetBoolean();

        object deviceDto = deviceType switch
        {
            "Embedded" => JsonSerializer.Deserialize<EmbeddedDTO>(content),
            "PersonalComputer" => JsonSerializer.Deserialize<PersonalComputerDTO>(content),
            "Smartwatch" => JsonSerializer.Deserialize<SmartwatchDTO>(content),
            _ => null
        };

        if (deviceDto == null)
            return Results.BadRequest("Invalid or unsupported device type.");

        deviceService.CreateDevice(deviceDto, deviceType, deviceName, isEnabled);
        return Results.Ok("Device created successfully!");
    }
    catch (JsonException ex)
    {
        return Results.BadRequest($"Invalid format: {ex.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unexpected error: {ex.Message}");
    }
});


app.MapPut("/api/devices/{deviceType}/{deviceId}", async (
    HttpRequest request,
    IDeviceService deviceService,
    string deviceType, string deviceId) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync();

    object deviceDto;

    try
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        deviceDto = deviceType switch
        {
            "Embedded"        => JsonSerializer.Deserialize<EmbeddedDTO>(content, options),
            "PersonalComputer"=> JsonSerializer.Deserialize<PersonalComputerDTO>(content, options),
            "Smartwatch"      => JsonSerializer.Deserialize<SmartwatchDTO>(content, options),
            _                 => throw new ArgumentException("Incorrect device type.")
        };


        if (deviceDto == null)
        {
            return Results.BadRequest("DTO is null");
        }

        deviceService.UpdateDevice(deviceId,deviceDto, deviceType);
        return Results.Ok("Device updated successfully :D");
    }
    catch (JsonException ex)
    {
        return Results.BadRequest($"JSON error: {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest($"Argument error: {ex.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Internal error: {ex.Message}");
    }
});

app.MapDelete("/api/devices/{deviceId}/{deviceType}", (string deviceId, string deviceType, IDeviceService deviceService) =>
{
    try
    {
        bool isDeleted = deviceService.DeleteDeviceById(deviceId, deviceType);
        if (isDeleted)
        {
            return Results.Ok("Device deleted successfully.");
        }
        else
        {
            return Results.NotFound("Device not found.");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


app.Run();