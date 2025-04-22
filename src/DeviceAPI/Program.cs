using task7;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();

FileService fileService = new FileService("/Users/aniasmuga/RiderProjects/Task5APBD/APBDlogic/input.txt");
DeviceParser deviceParser = new DeviceParser();
DeviceManager dm=DeviceManagerFactory.CreateDeviceManager(fileService, deviceParser);

app.MapGet("/api/devices", () => dm.GetAllDevices().Select((d) => new { Name = d.Name, Type = d.GetType().Name })
);
app.MapGet("/api/devices/{id}", (String id) => dm.GetDeviceById(id));

app.MapPost("/api/devices/smartwatch", (Smartwatch device) => { dm.AddDevice(device); });
app.MapPost("/api/devices/personalcomputer", (PersonalComputer device) => { dm.AddDevice(device); });
app.MapPost("/api/devices/Embedded", (Embedded device) => { dm.AddDevice(device); });

app.MapPut("/api/devices/smartwatch/{id}", (Smartwatch updatedDevice) =>
{ ;
    dm.EditDevice(updatedDevice);
});

app.MapPut("/api/devices/personalcomputer/{id}", (PersonalComputer updatedDevice) =>
{ ;
    dm.EditDevice(updatedDevice);
});

app.MapPut("/api/devices/embedded/{id}", (Embedded updatedDevice) =>
{ ;
    dm.EditDevice(updatedDevice);
});

app.MapDelete("/api/devices/{id}", (String id) =>
{
    dm.RemoveDeviceById(id);
});

app.Run();