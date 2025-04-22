using System.Text;

namespace task7;

/// <summary>
/// I created the FileService class to apply the single responibility principle
/// by taking away the responsibility of processing files form device manager class
/// </summary>
public class FileService:DataInterface
{
    private string _inputDeviceFile;

    public FileService(string filePath)
    {
        _inputDeviceFile = filePath;
        
    }

    public string[] GetDevices()
    {
        if (!File.Exists(_inputDeviceFile))
        {
            throw new FileNotFoundException("The input device file could not be found.");
        }

        var lines = File.ReadAllLines(_inputDeviceFile);
        return lines;
    }
    
    /// <summary>
    /// By previously adding the saveDevice method to the device classes
    /// I was able to create a more universal method for saving
    /// devices to a file (adheres to the open-closed principle)
    /// </summary>
    /// <param name="devices">
    /// A list of Device objects that we want to save to the file
    /// </param>
    public void SaveDevices(List<Device> devices)
    {
        StringBuilder devicesSb = new();

        foreach (var storedDevice in devices)
        {
            devicesSb.AppendLine(storedDevice.saveDevice());
        }

        File.WriteAllText("/Users/aniasmuga/RiderProjects/API/APBDlogic/input.txt", devicesSb.ToString());
    }


}