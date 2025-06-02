using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace TestConsole.Programs
{
    public class ChartBot
    {
        private List<Message> _botMessages = new List<Message>();
        private List<Message> _userMessages = new List<Message>();
        private Random _random = new Random();
        private List<ResponseKnowledge> responseKnowledges = new List<ResponseKnowledge>();
        private bool _isStartConversation => _userMessages.Count == 0;
        private static WordDictionary _wordDictionary = new WordDictionary();
        private BotState _botState = BotState.Awake;
        private EnumTrainingSession _trainSessionState = EnumTrainingSession.Home;
        public string BotName => "Zeus";

        public void Run()
        {
            try
            {
                while (true)
                {
                    _botMessages.Add(GetBotMessage());
                    _userMessages.Add(GetUserMessage());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The bot has run into an error: {ex}");
            }
        }

        private void TrainingSessionHandler(EnumTrainingSession trainingStatus)
        {
            switch (trainingStatus)
            {
                case EnumTrainingSession.Home:
                    TrainingSessionHomeHandler();
                    break;
                case EnumTrainingSession.MessageTypeIdentifing:
                    TrainingSessionMessageTypeIdentifingHandler();
                    break;
                default:
                    TrainingSessionHomeHandler();
                    break;
            }
        }

        private Message TrainingSessionResponser(EnumTrainingSession trainingStatus)
        {
            switch (trainingStatus)
            {
                case EnumTrainingSession.Home:
                    return TrainingSessionHomeResponser();
                case EnumTrainingSession.MessageTypeIdentifing:
                    return TrainingSessionMessageTypeIdentifingResponser();
                default:
                    return TrainingSessionHomeResponser();
            }
        }
        private bool TrainingSessionMessageTypeIdentifingHandler()
        {
            return true;
        }

        private bool TrainingSessionHomeHandler()
        {
            var userMessage = _userMessages.Last();
            if (userMessage.MessageText.Equals("1"))
                _trainSessionState = EnumTrainingSession.MessageTypeIdentifing;
            return true;
        }

        private Message TrainingSessionMessageTypeIdentifingResponser()
        {
            return BuildBotMessage("This is message-type identifying training");
        }

        private Message TrainingSessionHomeResponser()
        {
            var message =
                    $"Please type: \n" +
                    $" 1 For Message-Type-indentifying";
            return BuildBotMessage(message);
        }



        private Message GetUserMessage()
        {
            SetUserTextColor();
            Console.Write("\nUser: ");
            Console.ResetColor();
            return new Message
            {
                MessageText = Console.ReadLine() ?? "",
                IsBotMessage = false,
                MessageKeywords = new List<string>(),
                MessageType = MessageType.Default,
                PossibleResponses = new List<string>(),
            };
        }

        private void SetUserTextColor()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private Message GetBotMessage()
        {
            if (_botState == BotState.Training)
            {
                var botMessage = TrainingSession();
                BotReply(botMessage.MessageText);
                return botMessage;
            }
            if (_botState == BotState.Awake)
            {
                var botMessage = GenerateMessage();
                BotReply(botMessage.MessageText);
                return botMessage;
            }
            else
            {
                var botMessage = BuildBotMessage("I'm sorry, I can't help you right now. Please try again later.");
                BotReply(botMessage.MessageText);
                return botMessage;
            }
        }

        private void BotReply(string? messageText)
        {
            SetBotTextColor();
            Console.Write($"\nBot: ");
            Console.ResetColor();
            Console.WriteLine(messageText);
        }

        private void SetBotTextColor()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        private Message TrainingSession()
        {
            if (IsEndBotTraining())
            {
                _botState = BotState.Awake;
                _trainSessionState = EnumTrainingSession.Home;
                return BuildBotMessage("Training session has ended.");
            }
            TrainingSessionHandler(_trainSessionState);
            return TrainingSessionResponser(_trainSessionState);
        }

        private bool IsEndBotTraining()
        {
            var lastUserMessage = _userMessages.Last();
            return lastUserMessage.MessageText.Equals("End training", StringComparison.OrdinalIgnoreCase);
        }

        private Message GenerateMessage()
        {
            if (_isStartConversation)
                return BuildBotMessage("Hello! I'm an AI chatbot. How can I help you today?");
            if (IsCommandMessage())
            {
                _botState = BotState.Training;
                return BuildBotMessage("Command message has detected.");
            }

            var lastUserMessage = _userMessages.Last();
            IdentifyMessageType(lastUserMessage);
            ExtractKeyWords(lastUserMessage);
            MatchPossibleResponseWithKeywords(lastUserMessage);
            return SelectAResponseFromTheListOfPossibleResponses(lastUserMessage);
        }

        private bool IsCommandMessage()
        {
            var lastUserMessage = _userMessages.Last();
            return lastUserMessage.MessageText.Equals("Go training",StringComparison.OrdinalIgnoreCase);
        }

        private Message BuildBotMessage(string messageText)
        {
            var botMessage = new Message
            {
                MessageText = messageText,
                IsBotMessage = true,
                MessageKeywords = new List<string>(),
                MessageType = MessageType.Default,
                PossibleResponses = new List<string>(),
            };
            IdentifyMessageType(botMessage);
            ExtractKeyWords(botMessage);
            return botMessage;
        }

        public void CorrectTypo(Message message)
        {
            message.MessageText.Trim();
            var words = message.MessageText.Split(' ');
            var result = new StringBuilder();
            foreach (var word in words)
            {
                result.Append($"{_wordDictionary.CorrectWord(word)} ");
            }
            message.MessageText = result.ToString();
        }

        public void ExtractKeyWords(Message message)
        {
        }

        public void IdentifyMessageType(Message message)
        {
        }

        public void MatchPossibleResponseWithKeywords(Message message)
        {
        }

        public Message SelectAResponseFromTheListOfPossibleResponses(Message message)
        {
            return BuildBotMessage("Sorry, I have not been trained enough yet.");
        }
    }

    public class Message
    {
        public string? MessageText { get; set; }
        public List<string>? MessageKeywords { get; set; }
        public bool IsBotMessage { get; set; }
        public MessageType MessageType { get; set; }
        public List<string>? PossibleResponses { get; set; }
    }

    public class ResponseKnowledge
    {
        public MessageType MessageType { get; set; }
        public List<PossibleResponse>? PossibleResponses { get; set; }
    }

    public class PossibleResponse
    {
        public List<string>? KeyWords { get; set; }
        public string? MessageText { get; set; }
    }

    public class FileAccessHelper
    {
        private static string FolderPath => "C:\\Users\\sopheaktra.pin\\Documents";

        public static T ReadData<T>(string fileName) where T : class
        {
            var json = File.ReadAllText(Path.Combine(FolderPath, fileName));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void WriteData<T>(string fileName, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(Path.Combine(FolderPath, fileName), json);
        }
    }

    public class WordDictionary
    {
        private HashSet<string> dictionary;

        public WordDictionary()
        {
            dictionary = GetWords();
        }

        private HashSet<string> GetWords()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"words.txt");
            var wordsFromFile = File.ReadAllLines(path).ToHashSet<string>();
            var wordsForReturn = new HashSet<string>();
            foreach (var word in wordsFromFile)
            {
                wordsForReturn.Add(word.ToLower());
            }
            return wordsForReturn;
        }

        public string CorrectWord(string word)
        {
            if (dictionary.Contains(word.ToLower()))
            {
                return word;
            }

            var closestWord = dictionary
                .Select(dictWord => new { dictWord, distance = ComputeLevenshteinDistance(word, dictWord) })
                .OrderBy(pair => pair.distance)
                .FirstOrDefault();

            return closestWord?.dictWord ?? word;
        }

        private int ComputeLevenshteinDistance(string a, string b)
        {
            int n = a.Length;
            int m = b.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }

    public enum MessageType
    {
        Other,
        Answer,
        Question,
        Greeting,
        Help,
        ThankYou,
        Default,
        ConversationEnding
    }

    public enum BotState
    {
        Awake,
        Training,
        Sleeping
    }

    public enum EnumTrainingSession
    {
        Home,
        MessageTypeIdentifing,
    }
}