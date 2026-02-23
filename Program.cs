using System;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;

class Program
{
    // values: Disconnected, Connecting, Connected, Interrupted, Reconnecting, DisconnectingToReconnect, Disconnecting
    static string currentState = "Disconnected";
    static string currentRegion = "";
    static readonly object stateLock = new object();

    static readonly Dictionary<string, string> CountryCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        {"poland", "PL"},
        {"germany", "DE"},
        {"usa", "US"},
        {"uk", "UK"},
        {"france", "FR"},
        {"netherlands", "NL"},
        {"sweden", "SE"},
        {"switzerland", "CH"},
        {"japan", "JP"},
        {"singapore", "SG"},
        {"canada", "CA"},
        {"australia", "AU"},
        {"spain", "ES"},
        {"italy", "IT"},
        {"smart", "SMART"}
    };

    static void Main()
    {
        var stateProcess = StartProcess("expressvpnctl", "monitor connectionstate");
        var regionProcess = StartProcess("expressvpnctl", "monitor region");

        stateProcess.OutputDataReceived += (sender, args) => 
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (stateLock)
                {
                    currentState = args.Data.Trim();
                    UpdateWaybar();
                }
            }
        };

        regionProcess.OutputDataReceived += (sender, args) => 
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (stateLock)
                {
                    currentRegion = args.Data.Trim();
                    UpdateWaybar();
                }
            }
        };

        stateProcess.BeginOutputReadLine();
        regionProcess.BeginOutputReadLine();

        Thread.Sleep(Timeout.Infinite);
    }

    static Process StartProcess(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        return process;
    }

    static void UpdateWaybar()
    {
        string text;
        string cssClass;
        string tooltip = $"State: {currentState}";

        if (currentState == "Connected")
        {
            text = $"<span size='14000'></span> {GetCountryCode(currentRegion)}";
            tooltip += $"\nRegion: {currentRegion}";
            cssClass = "good";
        }
        else if (currentState == "Disconnected" || currentState == "Disconnecting")
        {
            text = "Disconnected";
            cssClass = "critical";
        }
        else
        {
            text = "Connecting...";
            cssClass = "warning";
        }

        var output = new
        {
            text = text,
            alt = currentState.ToLower(), // Waybar format-icons
            tooltip = tooltip,
            @class = cssClass
        };

        Console.WriteLine(JsonSerializer.Serialize(output));
    }

    static string GetCountryCode(string region)
    {
        if (string.IsNullOrWhiteSpace(region)) return "???";

        if (CountryCodes.TryGetValue(region, out string code))
        {
            return code;
        }

        // fallback
        return region.Length >= 2 ? region.Substring(0, 2).ToUpper() : region.ToUpper();
    }
}
