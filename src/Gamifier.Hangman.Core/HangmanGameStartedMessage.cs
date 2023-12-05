namespace Gamifier.Hangman.Core;

public record HangmanGameStartedMessage(int WordLength, int Attempts, Guid OwnerId) : HangmanMessage;