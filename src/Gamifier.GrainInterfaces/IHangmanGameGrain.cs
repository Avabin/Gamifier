using Orleans.Runtime;

namespace Gamifier.GrainInterfaces;

public interface IHangmanGameGrain : IGrainWithGuidKey
{
    Task StartGameAsync(string word, Guid caller, int attempts = 10);
    Task GuessAsync(char c, Guid caller);
}