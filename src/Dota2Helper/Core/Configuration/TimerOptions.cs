using System;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class TimerOptions
{
    [JsonRequired]
    public required string Speech { get; set; }
    
    [JsonRequired]
    public required string Label { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan First { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan Interval { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan Reminder { get; set; }

    /// <summary>
    ///  The audio file for effect sound
    /// </summary>
    [JsonRequired]
    public required string AudioFile { get; set; }

    /// <summary>
    /// If this timer is a manual reset timer, it will not automatically reset after it's up
    /// This is useful for timers that are not on a fixed interval (like Roshan timer or a custom timer)
    /// </summary>
    [JsonRequired]
    public required bool IsManualReset { get; set; }

    /// <summary>
    /// If the timer is enabled at all (if it's not enabled, it won't be started and will not show up in the Timer View UI)
    /// </summary>
    [JsonRequired]
    public required bool IsEnabled { get; set; }

    /// <summary>
    /// If text to speech or sound effect should be played when the timer is up
    /// </summary>
    [JsonRequired]
    public required bool IsTts { get; set; }

    /// <summary>
    /// If sound is enabled, this will be the sound file that will be played or the text that will be spoken
    /// </summary>
    [JsonRequired]
    public bool IsSoundEnabled { get; set; }
}