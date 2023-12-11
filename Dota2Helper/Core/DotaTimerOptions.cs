using System;
using System.ComponentModel;

namespace Dota2Helper.Core;

public class DotaTimerOptions
{
    public string Label { get; set; }
    public string First { get; set; }
    public string Interval { get; set; }
    public string Reminder { get; set; }
    public string AudioFile { get; set; }
    public bool IsManualReset { get; set; }
}