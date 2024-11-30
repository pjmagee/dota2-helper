using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dota2Helper.Features.Timers;

public class TimerStrategy
{
    public TimerStrategy()
    {
    }

    [SetsRequiredMembers]
    public TimerStrategy(string name, TimeMode mode, string toolTip)
    {
        Name = name;
        Mode = mode;
        ToolTip = toolTip;
    }

    public required string Name { get; set; }
    public required TimeMode Mode { get; set; }
    public required string ToolTip { get; set; }

    public readonly static List<TimerStrategy> Modes =
    [
        new(name: "Auto Switch", mode: TimeMode.Auto, toolTip: "Automatically switches between game and demo timers based on detecting if the game is running."),
        new(name: "Game Timer", mode: TimeMode.Real, toolTip: "Always use the game timer, even if the game is not running. No fake timer will run."),
        new(name: "Demo Timer", mode: TimeMode.Demo, toolTip: "Use the demo timer, even if the game is running. No real timer will run. Useful when the game is open but you are not in a match."),
    ];
}