using System.Collections.Generic;

namespace Dota2Helper.Features.Timers;

public class TimerStrategy
{
    public required string Name { get; set; }
    public TimeMode Mode { get; set; }
    public string ToolTip { get; set; }

    public static List<TimerStrategy> Modes =
    [
        new()
        {
            Name = "Auto Switch",
            Mode = TimeMode.Auto,
            ToolTip = "Automatically switches between game and demo timers based on detecting if the game is running."
        },

        new()
        {
            Name = "Game Timer",
            Mode = TimeMode.Real,
            ToolTip = "Always use the game timer, even if the game is not running. No fake timer will run."
        },

        new()
        {
            Name = "Demo Timer",
            Mode = TimeMode.Demo,
            ToolTip = "Use the demo timer, even if the game is running. No real timer will run. Useful when the game is open but you are not in a match."
        },
    ];
}