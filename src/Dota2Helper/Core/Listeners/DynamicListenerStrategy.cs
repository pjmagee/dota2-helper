using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;

namespace Dota2Helper.Core.Listeners;

public class DynamicListenerStrategy(List<IDotaListener> listeners) : IListenerStrategy
{
    public IDotaListener Current
    {
        get
        {
            if (Design.IsDesignMode)
            {
                return listeners.First(l => l.GetType() == typeof(FakeDotaListener));
            }

            if (Process.GetProcessesByName("dota2").Any())
            {
                return listeners.First(l => l.GetType() == typeof(DotaListener));
            }

            return listeners.First(l => l.GetType() == typeof(FakeDotaListener));
        }
    }
}