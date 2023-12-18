namespace Dota2Helper.Core.Configuration;

public class TimerOptions
{
    public string Speech { get; set; }
    public string Label { get; set; }
    public string First { get; set; }
    public string Interval { get; set; }
    public string Reminder { get; set; }
    public string AudioFile { get; set; }
    public bool IsManualReset { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsTts { get; set; }
}