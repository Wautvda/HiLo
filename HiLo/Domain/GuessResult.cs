using System.Text.Json.Serialization;

namespace HiLo.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<GuessResult>))]
public enum GuessResult
{
    /// <summary>
    /// Unknown?
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// The guess is correct!
    /// </summary>
    Correct,
    /// <summary>
    /// The Mystery Number is bigger than the guess.
    /// </summary>
    High,
    /// <summary>
    /// The Mystery Number is smaller than the guess.
    /// </summary>
    Low, 
}