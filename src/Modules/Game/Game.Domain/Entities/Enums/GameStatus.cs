namespace Games.Domain.Entities.Enums;

public enum GameStatus
{
    PendingConfirmation, // aguarda confirmação de todos os jogadores
    Confirmed,           // todos confirmaram, aguarda o dia/hora
    InProgress,          // jogo a decorrer
    Completed,           // resultado registado
    Cancelled            // jogo cancelado
}
