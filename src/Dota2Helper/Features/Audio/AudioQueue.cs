using System.Collections.Concurrent;

namespace Dota2Helper.Features.Audio;

public sealed class AudioQueue : ConcurrentQueue<string>;