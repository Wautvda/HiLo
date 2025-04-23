using HiLo.Domain;

namespace HiLo.Feature.Game.Guess;

public record GuessResponse(GuessResult Result, string Message)
{
    public static GuessResponse Create(GuessResult result, int totalGuesses)
    {
        var message = result switch
        {
            GuessResult.Correct => $"Congratulations! You guessed the correct number in {totalGuesses} attempts!"
            , GuessResult.High => $"Your guess is too low. Try again. Current attempts: {totalGuesses}"
            , GuessResult.Low => $"Your guess is too high. Try again. Current attempts: {totalGuesses}"
            , _ => "Unknown result."
        };
        return new GuessResponse(result, message);
    }
}