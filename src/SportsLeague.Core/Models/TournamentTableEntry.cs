namespace SportsLeague.Core.Models;

public class TournamentTableEntry
{
    public Guid TeamId { get; private set; }
    public string TeamName { get; private set; }
    public int Played { get; private set; }
    public int Wins { get; private set; }
    public int Draws { get; private set; }
    public int Losses { get; private set; }
    public int GoalsFor { get; private set; }
    public int GoalsAgainst { get; private set; }
    public int Points { get; private set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;

    public TournamentTableEntry(Guid teamId, string teamName)
    {
        TeamId = teamId;
        TeamName = teamName;
    }

    public void AddWin(int goalsFor, int goalsAgainst)
    {
        Played++;
        Wins++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        Points += 3;
    }

    public void AddDraw(int goalsFor, int goalsAgainst)
    {
        Played++;
        Draws++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        Points++;
    }

    public void AddLoss(int goalsFor, int goalsAgainst)
    {
        Played++;
        Losses++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
    }
}
