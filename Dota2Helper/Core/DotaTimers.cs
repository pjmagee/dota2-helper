using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Collections;

namespace Dota2Helper.Core;


public class DotaTimers : ObservableCollection<DotaTimer>
{
    public DotaTimers() : base(new List<DotaTimer>()
    {
        new WisdomTimer(
            fromGameStart: TimeSpan.FromMinutes(0),
            interval: TimeSpan.FromMinutes(7), 
            reminderTime: TimeSpan.FromSeconds(45)),
        
        new BountyTimer(
            fromGameStart: TimeSpan.FromMinutes(0),
            interval: TimeSpan.FromMinutes(3),
            reminderTime: TimeSpan.FromSeconds(20)),

        new PowerTimer(
            fromGameStart: TimeSpan.FromMinutes(0),
            interval: TimeSpan.FromMinutes(6),
            reminderTime: TimeSpan.FromSeconds(20)),
        
        new LotusTimer(
            fromGameStart: TimeSpan.FromMinutes(0),
            interval: TimeSpan.FromMinutes(3),
            reminderTime: TimeSpan.FromSeconds(15)),
        
        new DireTormentorTimer(
            fromGameStart: TimeSpan.FromMinutes(20), 
            interval: TimeSpan.FromMinutes(10),
            reminderTime: TimeSpan.FromSeconds(45)),
        
        new RadiantTormentorTimer(
            fromGameStart: TimeSpan.FromMinutes(20),
            interval: TimeSpan.FromMinutes(10),
            reminderTime: TimeSpan.FromSeconds(45)),
        
        new StackingTimer(
            fromGameStart: TimeSpan.FromMinutes(2),
            interval: TimeSpan.FromMinutes(1),
            reminderTime: TimeSpan.FromSeconds(15)),
        
        new RoshanTimer(
            fromGameStart: TimeSpan.FromMinutes(11),
            interval: TimeSpan.FromMinutes(11),
            reminderTime: TimeSpan.FromMinutes(3)),
        
        new CatapultTimer(
            fromGameStart: TimeSpan.FromMinutes(0),
            interval: TimeSpan.FromMinutes(5),
            reminderTime: TimeSpan.FromSeconds(30)),

        /*
         * -90 seconds in a regular match.
         * -75 seconds in a Play vs Bots match.
         * -60 seconds in a Turbo match.
         * 0 seconds in the Demo Hero mode.
         */
        
    }
        .OrderBy(x => x.Interval))
    {

    }

    public void Update(int index, DotaTimer timer)
    {
        SetItem(index, timer);
    }
}