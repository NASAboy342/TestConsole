using MiNET.Blocks;
using Newtonsoft.Json;

namespace TestConsole.Programs
{
    public class LlmPractice
    {
        public async Task Run()
        {
            var corpus = GetCorpus();
            var vocabulary = GetSeperatedLettersFromCorpus(corpus);
            var uniqueLetters = GetLettersAppearInCorpus(vocabulary);
            var pairLetterCount = GetPairLetterCount(vocabulary);
            var morstCountedLetters = GetMorstCountedLetters(pairLetterCount);
            AddMorstCountPairLettersIntoVocabulary(morstCountedLetters, vocabulary);
            Console.WriteLine(JsonConvert.SerializeObject(vocabulary));
        }

        private void AddMorstCountPairLettersIntoVocabulary(string morstCountedLetters, List<List<string>> vocabulary)
        {
            vocabulary.ForEach(word =>
            {
                word.ForEach(letter =>
                {
                    var currentLetterIndex = word.IndexOf(letter);
                    if (currentLetterIndex > 0)
                    {
                        var previousLetter = word[currentLetterIndex - 1];
                        if (previousLetter + letter == morstCountedLetters)
                        {
                            letter = morstCountedLetters;
                            previousLetter = "";
                        }
                    }
                });
                word.RemoveAll(letter => string.IsNullOrEmpty(letter) || letter.Length == 0);
            });
        }

        private string GetMorstCountedLetters(List<UniqueLetter> pairLetterCount)
        {
            return pairLetterCount.OrderByDescending(pairLetterCount => pairLetterCount.Count).FirstOrDefault()?.Character ?? string.Empty;
        }

        private List<UniqueLetter> GetPairLetterCount(List<List<string>> lettersSeperatedWords)
        {
            var uniqueLetters = new List<UniqueLetter>();
            foreach (var word in lettersSeperatedWords)
            {
                foreach (var letter in word)
                {
                    var currentLetterIndex = word.IndexOf(letter);
                    if (currentLetterIndex < 1)
                        continue;

                    var pairLetter = word[currentLetterIndex - 1] + letter;
                    CheckIfToCountOrAdd(uniqueLetters, pairLetter);
                }
            }

            return uniqueLetters.OrderByDescending(uniqueLetter => uniqueLetter.Count).ToList();
        }

        private static void CheckIfToCountOrAdd(List<UniqueLetter> uniqueLetters, string letter)
        {
            if (!uniqueLetters.Any(uniqueLetter => uniqueLetter.Character.Equals(letter)))
            {
                uniqueLetters.Add(new UniqueLetter
                {
                    Character = letter,
                    Count = 1
                });
            }
            else
            {
                var martchUniqueLetter = uniqueLetters.FirstOrDefault(uniqueLetter => uniqueLetter.Character.Equals(letter));
                if (martchUniqueLetter != null)
                {
                    martchUniqueLetter.Count++;
                }
            }
        }

        private List<UniqueLetter> GetLettersAppearInCorpus(List<List<string>> lettersSeperatedWords)
        {
            var uniqueLetters = new List<UniqueLetter>();
            foreach (var word in lettersSeperatedWords)
            {
                foreach (var letter in word)
                {
                    CheckIfToCountOrAdd(uniqueLetters, letter);
                }
            }
            
            return uniqueLetters.OrderByDescending(uniqueLetter => uniqueLetter.Count).ToList();
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
            return new List<string>()
            {
                "email",
                "emailing",
                "imagine",
                "imagined",
            };
        }
    }

    public class UniqueLetter
    {
        public string Character { get; set; }
        public int Count { get; set; }
    }
}