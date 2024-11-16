using System.Collections.Generic;

namespace Dota2Helper.Features.Timers;

public class TimerStrategy
{
    public required string Name { get; set; }
    public TimeMode Mode { get; set; }


    public static List<TimerStrategy> Modes =
    [
        new()
        {
            Name = "Auto Switch",
            Mode = TimeMode.Auto
        },

        new()
        {
            Name = "Game Timer",
            Mode = TimeMode.Auto
        },

        new()
        {
            Name = "Demo Timer",
            Mode = TimeMode.Demo
        },
    ];
}