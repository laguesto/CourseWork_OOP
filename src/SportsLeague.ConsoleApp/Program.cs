using System.Globalization;
using System.Text;
using SportsLeague.Core.Exceptions;
using SportsLeague.Core.Models;
using SportsLeague.Core.Repositories;
using SportsLeague.Core.Services;

namespace SportsLeague.ConsoleApp;

internal static class Program
{
    private static LeagueService _league = null!;
    private static JsonFileService _storage = null!;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        string defaultDataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        _storage = new JsonFileService(defaultDataDirectory);

        try
        {
            LoadData();
            Console.WriteLine("Дані успішно завантажено.");
            Console.WriteLine($"Папка з файлами: {_storage.DirectoryPath}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Не вдалося завантажити дані: {exception.Message}");
            Console.WriteLine("Перевірте JSON-файли або вкажіть іншу папку після перезапуску програми.");
            return;
        }

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Виберіть пункт: ", 0, 11);
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case 1:
                        AddTeam();
                        break;
                    case 2:
                        ShowTeams();
                        break;
                    case 3:
                        AddPlayer();
                        break;
                    case 4:
                        ShowPlayers();
                        break;
                    case 5:
                        ChangePlayerSuspension();
                        break;
                    case 6:
                        AddMatch();
                        break;
                    case 7:
                        ShowMatches();
                        break;
                    case 8:
                        ShowTournamentTable();
                        break;
                    case 9:
                        ChangeDataPath();
                        break;
                    case 10:
                        LoadDataWithConfirmation();
                        break;
                    case 11:
                        SaveData();
                        break;
                    case 0:
                        SaveData();
                        Console.WriteLine("Роботу програми завершено.");
                        isRunning = false;
                        break;
                }
            }
            catch (InvalidMatchResultException exception)
            {
                Console.WriteLine($"Некоректний результат матчу: {exception.Message}");
            }
            catch (PlayerSuspendedException exception)
            {
                Console.WriteLine($"Гравця не можна додати до складу: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Помилка: {exception.Message}");
            }

            if (isRunning)
            {
                Console.WriteLine();
                Console.WriteLine("Натисніть Enter, щоб продовжити...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("             СПОРТИВНА ЛІГА ТА ТУРНІРИ");
        Console.WriteLine("==================================================");
        Console.WriteLine("1. Додати команду");
        Console.WriteLine("2. Показати команди");
        Console.WriteLine("3. Додати гравця");
        Console.WriteLine("4. Показати гравців");
        Console.WriteLine("5. Змінити статус дискваліфікації гравця");
        Console.WriteLine("6. Додати результат матчу");
        Console.WriteLine("7. Показати матчі");
        Console.WriteLine("8. Показати турнірну таблицю");
        Console.WriteLine("9. Вказати шлях до папки з JSON-файлами");
        Console.WriteLine("10. Завантажити дані з вибраної папки");
        Console.WriteLine("11. Зберегти дані у вибрану папку");
        Console.WriteLine("0. Зберегти дані та вийти");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Поточна папка даних: {_storage.DirectoryPath}");
        Console.WriteLine("--------------------------------------------------");
    }

    private static void AddTeam()
    {
        Console.WriteLine("ДОДАВАННЯ КОМАНДИ");
        string name = ReadText("Назва команди: ");
        string city = ReadText("Місто: ");
        Team team = _league.AddTeam(name, city);
        Console.WriteLine($"Команду «{team.Name}» додано.");
    }

    private static void ShowTeams()
    {
        List<Team> teams = _league.GetTeams();
        Console.WriteLine("СПИСОК КОМАНД");

        if (teams.Count == 0)
        {
            Console.WriteLine("Команд поки немає.");
            return;
        }

        for (int i = 0; i < teams.Count; i++)
            Console.WriteLine($"{i + 1}. {teams[i].Name} — {teams[i].City}");
    }

    private static void AddPlayer()
    {
        Console.WriteLine("ДОДАВАННЯ ГРАВЦЯ");
        List<Team> teams = _league.GetTeams();
        if (teams.Count == 0)
        {
            Console.WriteLine("Спочатку додайте хоча б одну команду.");
            return;
        }

        Guid teamId = ChooseTeam(teams, "Виберіть команду гравця:");
        string fullName = ReadText("ПІБ гравця: ");
        string position = ReadText("Ігрова позиція: ");
        int jerseyNumber = ReadInt("Номер гравця: ", 1, 99);

        Player player = _league.AddPlayer(fullName, position, jerseyNumber, teamId);
        Console.WriteLine($"Гравця {player.FullName} додано.");
    }

    private static void ShowPlayers()
    {
        List<Player> players = _league.GetPlayers();
        List<Team> teams = _league.GetTeams();
        Console.WriteLine("СПИСОК ГРАВЦІВ");

        if (players.Count == 0)
        {
            Console.WriteLine("Гравців поки немає.");
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            string teamName = teams.FirstOrDefault(team => team.Id == player.TeamId)?.Name ?? "Команду не знайдено";
            string status = player.IsSuspended ? "[ДИСКВАЛІФІКОВАНИЙ]" : "";
            Console.WriteLine($"{i + 1}. №{player.JerseyNumber} {player.FullName}, {player.Position}, {teamName} {status}");
        }
    }

    private static void ChangePlayerSuspension()
    {
        List<Player> players = _league.GetPlayers();
        if (players.Count == 0)
        {
            Console.WriteLine("Гравців поки немає.");
            return;
        }

        Console.WriteLine("ВИБІР ГРАВЦЯ");
        for (int i = 0; i < players.Count; i++)
        {
            string status = players[i].IsSuspended ? "дискваліфікований" : "допущений до гри";
            Console.WriteLine($"{i + 1}. {players[i].FullName} — {status}");
        }

        int playerIndex = ReadInt("Номер гравця: ", 1, players.Count) - 1;
        Player selectedPlayer = players[playerIndex];
        bool suspend = ReadInt("1 — дискваліфікувати, 0 — допустити до гри: ", 0, 1) == 1;
        _league.SetPlayerSuspended(selectedPlayer.Id, suspend);
        Console.WriteLine("Статус гравця оновлено.");
    }

    private static void AddMatch()
    {
        Console.WriteLine("ДОДАВАННЯ РЕЗУЛЬТАТУ МАТЧУ");
        List<Team> teams = _league.GetTeams();
        if (teams.Count < 2)
        {
            Console.WriteLine("Для матчу потрібно додати щонайменше дві команди.");
            return;
        }

        Guid homeTeamId = ChooseTeam(teams, "Виберіть господарів поля:");
        Guid awayTeamId = ChooseTeam(teams, "Виберіть гостей поля:");

        int homeGoals = ReadInt("Кількість голів господарів: ", 0, 100);
        int awayGoals = ReadInt("Кількість голів гостей: ", 0, 100);
        DateTime matchDate = ReadDate("Дата матчу (РРРР-ММ-ДД, Enter — сьогодні): ");

        List<Guid> homePlayerIds = ReadLineup(homeTeamId, "Склад господарів");
        List<Guid> awayPlayerIds = ReadLineup(awayTeamId, "Склад гостей");

        Match match = _league.AddMatch(
            homeTeamId,
            awayTeamId,
            homeGoals,
            awayGoals,
            matchDate,
            homePlayerIds,
            awayPlayerIds);

        Console.WriteLine($"Матч додано. Результат: {homeGoals}:{awayGoals}.");
        Console.WriteLine($"Дата: {match.MatchDate:yyyy-MM-dd}");
    }

    private static List<Guid> ReadLineup(Guid teamId, string title)
    {
        List<Player> teamPlayers = _league.GetPlayers()
            .Where(player => player.TeamId == teamId)
            .ToList();

        Console.WriteLine();
        Console.WriteLine(title);

        if (teamPlayers.Count == 0)
        {
            Console.WriteLine("У цій команді поки немає гравців. Склад буде порожнім.");
            return new List<Guid>();
        }

        for (int i = 0; i < teamPlayers.Count; i++)
        {
            string status = teamPlayers[i].IsSuspended ? " [ДИСКВАЛІФІКОВАНИЙ]" : "";
            Console.WriteLine($"{i + 1}. {teamPlayers[i].FullName}{status}");
        }

        Console.Write("Введіть номери гравців через кому або Enter, щоб пропустити: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            return new List<Guid>();

        var selectedIds = new List<Guid>();
        string[] parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int number) || number < 1 || number > teamPlayers.Count)
                throw new ArgumentException("У складі вказано некоректні номери гравців.");

            Guid playerId = teamPlayers[number - 1].Id;
            if (!selectedIds.Contains(playerId))
                selectedIds.Add(playerId);
        }

        return selectedIds;
    }

    private static void ShowMatches()
    {
        List<Match> matches = _league.GetMatches()
            .OrderByDescending(match => match.MatchDate)
            .ToList();
        List<Team> teams = _league.GetTeams();

        Console.WriteLine("СПИСОК МАТЧІВ");
        if (matches.Count == 0)
        {
            Console.WriteLine("Матчів поки немає.");
            return;
        }

        foreach (Match match in matches)
        {
            string homeName = teams.FirstOrDefault(team => team.Id == match.HomeTeamId)?.Name ?? "Невідома команда";
            string awayName = teams.FirstOrDefault(team => team.Id == match.AwayTeamId)?.Name ?? "Невідома команда";
            Console.WriteLine($"{match.MatchDate:yyyy-MM-dd} | {homeName} {match.HomeGoals}:{match.AwayGoals} {awayName}");
        }
    }

    private static void ShowTournamentTable()
    {
        List<TournamentTableEntry> standings = _league.GetTournamentTable().Standings;
        Console.WriteLine("ТУРНІРНА ТАБЛИЦЯ");
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine("{0,3} {1,-22} {2,3} {3,3} {4,3} {5,3} {6,9} {7,5} {8,5}", "№", "Команда", "М", "В", "Н", "П", "З-П", "РГ", "Очки");
        Console.WriteLine("--------------------------------------------------------------------------------");

        for (int i = 0; i < standings.Count; i++)
        {
            TournamentTableEntry row = standings[i];
            string goals = $"{row.GoalsFor}:{row.GoalsAgainst}";
            Console.WriteLine($"{i + 1,3} {row.TeamName,-22} {row.Played,3} {row.Wins,3} {row.Draws,3} {row.Losses,3} {goals,9} {row.GoalDifference,5} {row.Points,5}");
        }

        if (standings.Count == 0)
            Console.WriteLine("Команд поки немає.");

        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine("М — матчі; В — перемоги; Н — нічиї; П — поразки; РГ — різниця голів.");
    }

    private static Guid ChooseTeam(List<Team> teams, string prompt)
    {
        Console.WriteLine(prompt);
        for (int i = 0; i < teams.Count; i++)
            Console.WriteLine($"{i + 1}. {teams[i].Name} ({teams[i].City})");

        int index = ReadInt("Номер команди: ", 1, teams.Count) - 1;
        return teams[index].Id;
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Введіть ціле число від {min} до {max}.");
        }
    }

    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            Console.WriteLine("Поле не може бути порожнім.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return DateTime.Today;

            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
                return date;

            Console.WriteLine("Введіть дату у форматі РРРР-ММ-ДД.");
        }
    }

    private static void ChangeDataPath()
    {
        Console.WriteLine("ЗМІНА ШЛЯХУ ДО JSON-ФАЙЛІВ");
        Console.WriteLine("Вкажіть шлях до папки, у якій зберігатимуться teams.json, players.json та matches.json.");
        Console.WriteLine("Приклад: C:\\Users\\User\\Documents\\SportsLeagueData");

        string enteredPath = ReadText("Шлях до папки: ").Trim().Trim('"');
        string fullPath = Path.GetFullPath(enteredPath);

        _storage = new JsonFileService(fullPath);
        Console.WriteLine("Шлях успішно змінено.");
        Console.WriteLine($"Теперішня папка даних: {_storage.DirectoryPath}");
        Console.WriteLine("Щоб прочитати файли з цієї папки, виберіть пункт 10.");
    }

    private static void LoadDataWithConfirmation()
    {
        Console.WriteLine("ЗАВАНТАЖЕННЯ ДАНИХ З JSON");
        Console.WriteLine($"Джерело: {_storage.DirectoryPath}");
        Console.WriteLine("Увага: завантаження замінить поточні дані в пам'яті програми.");

        if (!ReadYesNo("Продовжити? (т/н): "))
        {
            Console.WriteLine("Завантаження скасовано.");
            return;
        }

        LoadData();
        Console.WriteLine("Дані успішно завантажено.");
        Console.WriteLine($"Команд: {_league.GetTeams().Count}; гравців: {_league.GetPlayers().Count}; матчів: {_league.GetMatches().Count}.");
    }

    private static void LoadData()
    {
        // Спочатку читаємо всі файли. Якщо один із них пошкоджений, поточні дані не зміняться.
        List<Team> teams = _storage.Load<Team>("teams.json");
        List<Player> players = _storage.Load<Player>("players.json");
        List<Match> matches = _storage.Load<Match>("matches.json");

        _league = new LeagueService(
            new Repository<Team>(teams),
            new Repository<Player>(players),
            new Repository<Match>(matches));
    }

    private static void SaveData()
    {
        _storage.Save("teams.json", _league.GetTeams());
        _storage.Save("players.json", _league.GetPlayers());
        _storage.Save("matches.json", _league.GetMatches());

        Console.WriteLine("Дані збережено у JSON-файли.");
        Console.WriteLine($"Папка: {_storage.DirectoryPath}");
    }

    private static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? answer = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (answer is "т" or "так" or "y" or "yes")
                return true;

            if (answer is "н" or "ні" or "n" or "no")
                return false;

            Console.WriteLine("Введіть «т» або «н».");
        }
    }
}
