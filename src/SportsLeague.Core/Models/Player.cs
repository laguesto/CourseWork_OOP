namespace SportsLeague.Core.Models;

public class Player : IEntity
{
    private string _fullName = string.Empty;
    private string _position = string.Empty;
    private int _jerseyNumber;
    private Guid _teamId;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім’я гравця не може бути порожнім.");

            _fullName = value.Trim();
        }
    }

    public string Position
    {
        get => _position;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Вкажіть ігрову позицію гравця.");

            _position = value.Trim();
        }
    }

    public int JerseyNumber
    {
        get => _jerseyNumber;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Номер гравця має бути більшим за нуль.");

            _jerseyNumber = value;
        }
    }

    public Guid TeamId
    {
        get => _teamId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Для гравця потрібно вибрати команду.");

            _teamId = value;
        }
    }

    public bool IsSuspended { get; set; }

    public Player()
    {
    }

    public Player(string fullName, string position, int jerseyNumber, Guid teamId)
    {
        FullName = fullName;
        Position = position;
        JerseyNumber = jerseyNumber;
        TeamId = teamId;
    }
}
