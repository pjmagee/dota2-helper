using System.Threading;
using System.Threading.Tasks;

namespace Dota2Helper.Core.Listeners;

public interface IListenerStrategy
{
    IDotaListener Listener { get; }
    void UpdateListener();
}