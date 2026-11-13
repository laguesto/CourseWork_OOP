namespace SportsLeague.Core.Models;

public class Match : IEntity
{
    private Guid _homeTeamId;
    private Guid _awayTeamId;
    private int _homeGoals;
    private int _awayGoals;

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HomeTeamId
    {
        get => _homeTeamId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Не вибрано команду господарів поля.");

            _homeTeamId = value;
        }
    }

    public Guid AwayTeamId
    {
        get => _awayTeamId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Не вибрано команду гостей поля.");

            _awayTeamId = value;
        }
    }

    public int HomeGoals
    {
        get => _homeGoals;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(HomeGoals), "Кількість голів не може бути від’ємною.");

            _homeGoals = value;
        }
    }

    public int AwayGoals
    {
        get => _awayGoals;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(AwayGoals), "Кількість голів не може бути від’ємною.");

            _awayGoals = value;
        }
    }

    public DateTime MatchDate { get; set; } = DateTime.Today;

    public List<Guid> HomePlayerIds { get; set; } = new();
    public List<Guid> AwayPlayerIds { get; set; } = new();

    public Match()
    {
    }

    public Match(
        Guid homeTeamId,
        Guid awayTeamId,
        int homeGoals,
        int awayGoals,
        DateTime matchDate,
        List<Guid> homePlayerIds,
        List<Guid> awayPlayerIds)
    {
        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Команди в матчі мають бути різними.");

        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        MatchDate = matchDate;
        HomePlayerIds = homePlayerIds;
        AwayPlayerIds = awayPlayerIds;
    }
}
