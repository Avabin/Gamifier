namespace Gamifier.Hangman.Core;

public record HangmanGameLostMessage(string Word, Guid LoserId) : HangmanMessage;