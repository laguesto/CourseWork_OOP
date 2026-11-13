namespace SportsLeague.Core.Exceptions;

public class InvalidMatchResultException : Exception
{
    public InvalidMatchResultException(string message) : base(message)
    {
    }
}
