namespace SportsLeague.Core.Models;

public class Team : IEntity
{
    private string _name = string.Empty;
    private string _city = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва команди не може бути порожньою.");

            _name = value.Trim();
        }
    }

    public string City
    {
        get => _city;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Вкажіть місто команди.");

            _city = value.Trim();
        }
    }

    // Порожній конструктор потрібен для десеріалізації JSON.
    public Team()
    {
    }

    public Team(string name, string city)
    {
        Name = name;
        City = city;
    }
}
