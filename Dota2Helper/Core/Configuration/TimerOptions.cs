namespace Dota2Helper.Core.Configuration;

public class TimerOptions
{
    public required string Speech { get; set; }
    public required string Label { get; set; }
    public required string First { get; set; }
    public required string Interval { get; set; }
    public required string Reminder { get; set; }
    public required string AudioFile { get; set; }
    public required bool IsManualReset { get; set; }
    public required bool IsEnabled { get; set; }
    public required bool IsTts { get; set; }
}