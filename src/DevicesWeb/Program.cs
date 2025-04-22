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
app.MapPost("/api/devices/{deviceType}/{deviceName}/{isEnabled}", async (HttpRequest request, IDeviceService deviceService, string deviceType, string deviceName, bool isEnabled) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync();

    object deviceDto;

    try
    {
        switch (deviceType)
        {
            case "Embedded":
                deviceDto = JsonSerializer.Deserialize<EmbeddedDTO>(content);
                break;

            case "PersonalComputer":
                deviceDto = JsonSerializer.Deserialize<PersonalComputerDTO>(content);
                break;

            case "Smartwatch":
                deviceDto = JsonSerializer.Deserialize<SmartwatchDTO>(content);
                break;

            default:
                return Results.BadRequest("invalid device Type.");
        }

        var device = new DeviceDTO
        {
            Name = deviceName,
            IsEnabled = isEnabled
        };

        if (deviceDto is EmbeddedDTO embeddedDto)
        {
            embeddedDto.Device = device;
        }
        else if (deviceDto is PersonalComputerDTO pcDto)
        {
            pcDto.Device = device;
        }
        else if (deviceDto is SmartwatchDTO smartwatchDto)
        {
            smartwatchDto.Device = device;
        }
        else
        {
            return Results.BadRequest(":(");
        }
    }
    catch (JsonException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
    deviceService.CreateDevice(deviceDto, deviceType, deviceName, isEnabled);

    return Results.Ok("Device created successfully :D");
});

app.MapPut("/api/devices/{deviceType}", async (HttpRequest request, IDeviceService deviceService, string deviceType) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync();

    object deviceDto;

    try
    {
        switch (deviceType)
        {
            case "Embedded":
                deviceDto = JsonSerializer.Deserialize<EmbeddedDTO>(content);
                break;

            case "PersonalComputer":
                deviceDto = JsonSerializer.Deserialize<PersonalComputerDTO>(content);
                break;

            case "Smartwatch":
                deviceDto = JsonSerializer.Deserialize<SmartwatchDTO>(content);
                break;

            default:
                return Results.BadRequest("Incorrect device type.");
        }

        deviceService.UpdateDevice(deviceDto, deviceType);

        return Results.Ok("Device updated successfully :D");
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
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