using task7;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

FileService fileService = new FileService("/Users/aniasmuga/Desktop/APBD/tutorial7/src/DevicesLogic/input.txt");
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