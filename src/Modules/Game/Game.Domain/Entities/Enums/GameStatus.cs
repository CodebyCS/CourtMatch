namespace Game.Domain.Entities.Enums;

public enum GameStatus
{
    PendingConfirmation, // awaiting confirmation from all players
    Confirmed,           // everyone has confirmed; awaiting the date and time.
    InProgress,          // match in progress
    Completed,           // recorded result
    Cancelled            // match cancelled
}
