using System.Runtime.Serialization;

namespace Gamifier.Hangman.Core;

[DataContract]
public record HangmanGameGuessedWrongMessage(
    
    char Letter, int RemainingGuesses, Guid GuesserId) : HangmanMessage;