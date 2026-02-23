using System;
using System.Diagnostics;
using System.Text.Json;

class Program
{
    static void Main()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "expressvpnctl",
            Arguments = "monitor region",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        
        try
        {
            process.Start();
            
            while (!process.StandardOutput.EndOfStream)
            {
                string? line = process.StandardOutput.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var output = new
                    {
                        text = line.Trim(),
                        alt = line.Trim() == "smart" ? "disconnected" : "connected",
                        tooltip = $"VPN Location: {line}",
                        @class = line.Trim() == "smart" ? "warning" : "good"
                    };
                    Console.WriteLine(JsonSerializer.Serialize(output));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(JsonSerializer.Serialize(new { text = "Error", tooltip = ex.Message }));
        }
    }
}
