namespace SportsLeague.Core.Models;

public class TournamentTable
{
    public List<TournamentTableEntry> Standings { get; set; } = new();

    public static TournamentTable Create(IReadOnlyCollection<Team> teams, IReadOnlyCollection<Match> matches)
    {
        var entries = teams.ToDictionary(
            team => team.Id,
            team => new TournamentTableEntry(team.Id, team.Name));

        foreach (Match match in matches)
        {
            if (!entries.TryGetValue(match.HomeTeamId, out TournamentTableEntry? home) ||
                !entries.TryGetValue(match.AwayTeamId, out TournamentTableEntry? away))
            {
                continue;
            }

            if (match.HomeGoals > match.AwayGoals)
            {
                home.AddWin(match.HomeGoals, match.AwayGoals);
                away.AddLoss(match.AwayGoals, match.HomeGoals);
            }
            else if (match.HomeGoals < match.AwayGoals)
            {
                home.AddLoss(match.HomeGoals, match.AwayGoals);
                away.AddWin(match.AwayGoals, match.HomeGoals);
            }
            else
            {
                home.AddDraw(match.HomeGoals, match.AwayGoals);
                away.AddDraw(match.AwayGoals, match.HomeGoals);
            }
        }

        // Спочатку враховуються очки, потім різниця голів і кількість забитих голів.
        List<TournamentTableEntry> orderedEntries = entries.Values
            .OrderByDescending(entry => entry.Points)
            .ThenByDescending(entry => entry.GoalDifference)
            .ThenByDescending(entry => entry.GoalsFor)
            .ThenBy(entry => entry.TeamName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new TournamentTable { Standings = orderedEntries };
    }
}
