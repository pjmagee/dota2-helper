using System;

namespace D2Helper.Models;

public class DotaTimer
{
    public bool IsEnabled { get; set; }
    public bool IsManualReset { get; set; }
    public bool IsMuted { get; set; }
    public bool IsInterval { get; set; }

    public string Name { get; set; }
    public string? Speech { get; set; }
    public string? AudioFile { get; set; }

    public TimeSpan Every { get; set; }
    public TimeSpan? RemindAt { get; set; }
    public TimeSpan? DisableAfter { get; set; }
    public TimeSpan? Starts { get; set; }
}

// Example appsettings.json for the Timer:

/*

{
  "Settings": {
    "Timers": [
      {
        "IsEnabled": true,
        "IsManualReset": false,
        "IsMuted": false,
        "IsInterval": false,
        "Name": "Roshan",
        "Speech": "Roshan is up",
        "AudioFile": "roshan.wav",
        "Every": "0:08:00",
        "RemindAt": "0:07:00",
        "DisableAfter": "0:00:00",
        "Starts": "0:00:00"
      },
      {
        "IsEnabled": true,
        "IsManualReset": false,
        "IsMuted": false,
        "IsInterval": false,
        "Name": "Aegis",
        "Speech": "Aegis is up",
        "AudioFile": "aegis.wav",
        "Every": "0:05:00",
        "RemindAt": "0:04:00",
        "DisableAfter": "0:00:00",
        "Starts": "0:00:00"
      },
      {
        "IsEnabled": true,
        "IsManualReset": false,
        "IsMuted": false,
        "IsInterval": false,
        "Name": "Bounty",
        "Speech": "Bounty is up",
        "AudioFile": "bounty.wav",
        "Every": "0:05:00",
        "RemindAt": "0:04:00",
        "DisableAfter": "0:00:00",
        "Starts": "0:00:00"
      }
    ],
    "Volume": 50,
    "Mode": 0,
    "DemoMuted": true
  }
}

*/