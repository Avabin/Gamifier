namespace Gamifier.GrainInterfaces;

public interface IHangmanGamePlayerGrain : IGrainWithGuidKey
{
    Task JoinGameAsync(IHangmanGameGrain game);
    Task LeaveGameAsync();
    
    Task GuessAsync(char c);
}