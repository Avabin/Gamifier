using System.Runtime.Serialization;

namespace Gamifier.Hangman.Core;

[DataContract]
public record HangmanGameGuessedCorrectMessage(
    [DataMember] string DisplayWord,
    [DataMember] char Letter,
    [DataMember] Guid GuesserId) 
    : HangmanMessage;