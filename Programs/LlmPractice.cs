using MiNET.Blocks;
using Newtonsoft.Json;

namespace TestConsole.Programs
{
    public class LlmPractice
    {
        private static string _modelPath = "C:\\Users\\Asus\\Downloads\\Model.txt";
        private static string _dataPath = "C:\\Users\\Asus\\Downloads\\Data.txt";
        private static int _tokenizeIterateTime = 2;
        public async Task Run()
        {
            //RunTokenization();
            UseTheModel();
        }

        private void UseTheModel()
        {
            while (true)
            {
                Console.Clear();
                var models = GetModel();
                Console.Write("What is you init:");
                var initalize = Console.ReadLine() ?? string.Empty;
                DoPrediction(models, 100, initalize);
                Console.ReadKey();
            }
        }

        private void RunTokenization()
        {
            List<PairCorpus> tokens = Tokenization();
            using var writer = new StreamWriter(_modelPath);
            var json = JsonConvert.SerializeObject(tokens, Formatting.Indented);
            writer.Write(json);
        }

        private void DoPrediction(List<PairCorpus> models, int iteration, string initalize)
        {
            var predictions = new List<string>();
            predictions.Add(initalize);

            for (int i = 1; i < iteration; i++)
            {
                var lastToken = predictions[i-1] ?? string.Empty;

                var possibleNext = models
                    .Where(m => m.FirstPart == lastToken)
                    .OrderByDescending(m => m.Count)
                    .ToList();

                if (possibleNext.Count == 0)
                    break;

                var next = new PairCorpus();
                if(possibleNext.Count > 1)
                {
                    var isTakeTheSecondIndex = new Random().Next(0, 10) <= 8;
                    next = isTakeTheSecondIndex ? possibleNext[1] : possibleNext[0];
                }
                else
                {
                    next = possibleNext.First();
                }

                string toAppend = next.SecondPart ?? "";

                predictions.Add(toAppend);
            }

            Console.Write("Prediction: ");
            foreach (var prediction in predictions)
            {
                Console.Write(prediction + " ");
            }
        }

        private List<PairCorpus> GetModel()
        {
            using var reader = new StreamReader(_modelPath);
            var content = reader.ReadToEnd();
            var models = JsonConvert.DeserializeObject<List<PairCorpus>>(content) ?? new List<PairCorpus>();
            return models;
        }

        private List<PairCorpus> Tokenization()
        {
            var tokenizeIterateTime = _tokenizeIterateTime;
            var corpus = GetCorpus();
            tokenizeIterateTime = corpus.Count;
            var tokens = new List<PairCorpus>();
            for( int iteration = 0; iteration < tokenizeIterateTime-1; iteration++)
            {
                var pairCorpus = new PairCorpus()
                {
                    Count = 1,
                    FirstPart = corpus[iteration].ToLower(),
                    SecondPart = corpus[iteration + 1].ToLower()
                };
                var existingPair = tokens.FirstOrDefault(t => t.FirstPart == pairCorpus.FirstPart && t.SecondPart == pairCorpus.SecondPart);
                if (existingPair != null)
                {
                    existingPair.Count++;
                }
                else
                {
                    tokens.Add(pairCorpus);
                }
            }
            return tokens;
        }


        private void AddMorstCountPairLettersIntoVocabulary(string morstCountedLetters, List<List<string>> vocabulary)
        {
            for(var wordIndex = 0; wordIndex < vocabulary.Count;  wordIndex++)
            {
                var word = vocabulary[wordIndex];
                for(var letterIndex = 0; letterIndex < word.Count; letterIndex++)
                {
                    var letter = word[letterIndex];
                    var currentLetterIndex = letterIndex;
                    if (currentLetterIndex > 0)
                    {
                        var previousLetter = word[currentLetterIndex - 1];
                        if (previousLetter + letter == morstCountedLetters)
                        {
                            word[letterIndex] = morstCountedLetters;
                            word[currentLetterIndex - 1] = "";
                        }
                    }
                }
                word.RemoveAll(letter => string.IsNullOrEmpty(letter) || letter.Length == 0);
            }
        }

        private static List<List<string>> GetSeperatedLettersFromCorpus(List<string> corpus)
        {
            var words = new List<List<string>>();
            foreach (var word in corpus)
            {
                var letters = new List<string>();
                foreach (var letter in word)
                {
                    letters.Add(letter.ToString());
                }
                words.Add(letters);
            }
            return words;
        }

        private static List<string> GetCorpus()
        {
            using var reader = new StreamReader(_dataPath);
            var data = reader.ReadToEnd();
            var corpus = data.Split(' ').ToList();
            return corpus;
        }
    }

    public class PairCorpus
    {
        public int Count { get; set; }
        public string? FirstPart { get; set; }
        public string? SecondPart { get; set; }
    }
}