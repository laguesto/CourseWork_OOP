namespace SportsLeague.Core.Exceptions;

public class PlayerSuspendedException : Exception
{
    public string PlayerName { get; }

    public PlayerSuspendedException(string playerName)
        : base($"Гравець {playerName} дискваліфікований і не може брати участь у матчі.")
    {
        PlayerName = playerName;
    }
}
