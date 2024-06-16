namespace Dota2Helper.Core.Listeners;

public interface IListenerStrategy
{
    IDotaListener Current { get; }
}