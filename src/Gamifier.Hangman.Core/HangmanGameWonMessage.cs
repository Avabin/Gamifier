namespace Gamifier.Hangman.Core;

public record HangmanGameWonMessage(string Word, Guid WinnerId) : HangmanMessage;