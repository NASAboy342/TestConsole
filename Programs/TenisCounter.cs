namespace TestConsole.Programs
{
    internal class TenisCounter
    {
        public static void Run()
        {
            var players = new List<Player>();
            GetPlayerName(players);
            ShowPlayerName(players);
            GameStart(players);
        }

        private static void ShowPlayerName(List<Player> players)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"Player 1: {players[0].Name} | Player 2: {players[1].Name}");
            Console.WriteLine("------------------");
        }

        private static void GameStart(List<Player> players)
        {
            Console.WriteLine("GAME START");
            Console.WriteLine("=========================");
            Console.WriteLine("Input Player score. | Type '1' For player 1 | Type '2' For player 2 |");
            Console.WriteLine("=========================");
            GetScore(players);
        }

        private static void GetScore(List<Player> players)
        {
            while (!IsAnyScoreBiggerThan3(players) && !BothScoreEqual3(players))
            {
                ShowScore(players);
                AskForInput(players);
            }
        }

        private static void AskForInput(List<Player> players)
        {
            Console.Write("Input Player score. : ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    players[0].Score++;
                    break;

                case "2":
                    players[1].Score++;
                    break;

                default:
                    break;
            }
        }

        private static void ShowScore(List<Player> players)
        {
            Console.WriteLine("--------------------");
            Console.WriteLine($"{GetScoreInNumber(players)} | {GetScoreInWord(players)}");
            Console.WriteLine("--------------------");
        }

        private static object GetScoreInWord(List<Player> players)
        {
            return "";
        }

        private static object GetScoreInNumber(List<Player> players)
        {
            var scoreDictionary = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 15},
            { 2, 30},
            { 3, 40}
        };
            return $" {scoreDictionary[players[0].Score]} - {scoreDictionary[players[1].Score]}";
        }

        private static bool BothScoreEqual3(List<Player> players)
        {
            return players[0].Score == 3 && players[1].Score == 3;
        }

        private static bool IsAnyScoreBiggerThan3(List<Player> players)
        {
            return players.Any(player => player.Score > 3);
        }

        private static void GetPlayerName(List<Player> players)
        {
            Console.WriteLine("Welcome to the game. Please enter the player Name.");
            Console.Write("Player 1: ");
            players.Add(new Player() { Name = Console.ReadLine() });
            Console.Write("Player 2: ");
            players.Add(new Player() { Name = Console.ReadLine() });
        }
    }

    internal class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}