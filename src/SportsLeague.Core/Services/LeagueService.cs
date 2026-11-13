using SportsLeague.Core.Exceptions;
using SportsLeague.Core.Interfaces;
using SportsLeague.Core.Models;

namespace SportsLeague.Core.Services;

public class LeagueService
{
    private readonly IRepository<Team> _teams;
    private readonly IRepository<Player> _players;
    private readonly IRepository<Match> _matches;

    public LeagueService(
        IRepository<Team> teams,
        IRepository<Player> players,
        IRepository<Match> matches)
    {
        _teams = teams;
        _players = players;
        _matches = matches;
    }

    public List<Team> GetTeams() => _teams.GetAll();
    public List<Player> GetPlayers() => _players.GetAll();
    public List<Match> GetMatches() => _matches.GetAll();

    public Team AddTeam(string name, string city)
    {
        var team = new Team(name, city);

        bool nameExists = _teams.Find(existingTeam =>
            string.Equals(existingTeam.Name, team.Name, StringComparison.OrdinalIgnoreCase)).Count > 0;

        if (nameExists)
            throw new InvalidOperationException("Команда з такою назвою вже існує.");

        _teams.Add(team);
        return team;
    }

    public Player AddPlayer(string fullName, string position, int jerseyNumber, Guid teamId)
    {
        if (_teams.GetById(teamId) == null)
            throw new InvalidOperationException("Вибрану команду не знайдено.");

        bool numberExists = _players.Find(player =>
            player.TeamId == teamId && player.JerseyNumber == jerseyNumber).Count > 0;

        if (numberExists)
            throw new InvalidOperationException("У цій команді вже є гравець із таким номером.");

        var player = new Player(fullName, position, jerseyNumber, teamId);
        _players.Add(player);
        return player;
    }

    public void SetPlayerSuspended(Guid playerId, bool isSuspended)
    {
        Player? player = _players.GetById(playerId);

        if (player == null)
            throw new InvalidOperationException("Гравця не знайдено.");

        player.IsSuspended = isSuspended;
    }

    public Match AddMatch(
        Guid homeTeamId,
        Guid awayTeamId,
        int homeGoals,
        int awayGoals,
        DateTime matchDate,
        IEnumerable<Guid> homePlayerIds,
        IEnumerable<Guid> awayPlayerIds)
    {
        if (_teams.GetById(homeTeamId) == null || _teams.GetById(awayTeamId) == null)
            throw new InvalidOperationException("Одну з вибраних команд не знайдено.");

        if (homeTeamId == awayTeamId)
            throw new InvalidMatchResultException("Не можна створити матч між однією й тією самою командою.");

        if (homeGoals < 0 || awayGoals < 0)
            throw new InvalidMatchResultException("Рахунок матчу не може містити від’ємні значення.");

        List<Guid> homeIds = homePlayerIds.Distinct().ToList();
        List<Guid> awayIds = awayPlayerIds.Distinct().ToList();

        ValidateLineup(homeTeamId, homeIds);
        ValidateLineup(awayTeamId, awayIds);

        var match = new Match(
            homeTeamId,
            awayTeamId,
            homeGoals,
            awayGoals,
            matchDate,
            homeIds,
            awayIds);

        _matches.Add(match);
        return match;
    }

    public TournamentTable GetTournamentTable()
    {
        return TournamentTable.Create(_teams.GetAll(), _matches.GetAll());
    }

    private void ValidateLineup(Guid teamId, List<Guid> playerIds)
    {
        foreach (Guid playerId in playerIds)
        {
            Player? player = _players.GetById(playerId);

            if (player == null)
                throw new InvalidOperationException("Одного з вибраних гравців не знайдено.");

            if (player.TeamId != teamId)
                throw new InvalidOperationException("Гравець має виступати за вибрану команду.");

            if (player.IsSuspended)
                throw new PlayerSuspendedException(player.FullName);
        }
    }
}
