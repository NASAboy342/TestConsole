using MiNET.Blocks;
using Newtonsoft.Json;

namespace TestConsole.Programs
{
    public class LlmPractice
    {
        public async Task Run()
        {
            List<UniqueLetter> tokens = Tokenization();
            Console.WriteLine(JsonConvert.SerializeObject(tokens));
        }

        private List<UniqueLetter> Tokenization()
        {
            var tokenizeIterateTime = 30;
            var corpus = GetCorpus();
            tokenizeIterateTime = corpus.Count;
            var vocabulary = GetSeperatedLettersFromCorpus(corpus);
            var tokens = GetLettersAppearInCorpus(vocabulary);
            for (var tokenizeTime = 1; tokenizeTime <= tokenizeIterateTime; tokenizeTime++)
            {
                var pairLetterCount = GetPairLetterCount(vocabulary);
                var morstCountedLetters = GetMorstCountedLetters(pairLetterCount);
                AddMorstCountPairLettersIntoVocabulary(morstCountedLetters, vocabulary);
                UpdateToken(pairLetterCount, tokens);
            }

            return tokens;
        }

        private void UpdateToken(List<UniqueLetter> pairLetterCount, List<UniqueLetter> tokens)
        {
            var topCountedPair = pairLetterCount.OrderByDescending(p => p.Count).FirstOrDefault();
            if (topCountedPair == null)
                return;
            var firstPartMarch = tokens.FirstOrDefault(token => token.Character.Equals(topCountedPair.FirstPart));
            firstPartMarch.Count -= topCountedPair.Count;

            var secondPartMarch = tokens.FirstOrDefault(token => token.Character.Equals(topCountedPair.SecondPart));
            secondPartMarch.Count -= topCountedPair.Count;

            tokens.Add(topCountedPair);
            tokens.RemoveAll(token => token.Count == 0);
            tokens = tokens.OrderBy(token => token.Character).ToList();
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

        private string GetMorstCountedLetters(List<UniqueLetter> pairLetterCount)
        {
            return pairLetterCount.OrderByDescending(pairLetterCount => pairLetterCount.Count).FirstOrDefault()?.Character ?? string.Empty;
        }

        private List<UniqueLetter> GetPairLetterCount(List<List<string>> lettersSeperatedWords)
        {
            var uniqueLetters = new List<UniqueLetter>();
            foreach (var word in lettersSeperatedWords)
            {
                for(var letterIndex = 0; letterIndex < word.Count; letterIndex++)
                {
                    var letter = word[letterIndex];
                    if (letterIndex < 1)
                        continue;

                    var pairLetter = word[letterIndex - 1] + letter;
                    CheckIfToCountOrAdd(uniqueLetters, pairLetter, word[letterIndex - 1], letter);
                }
            }

            return uniqueLetters.OrderByDescending(uniqueLetter => uniqueLetter.Count).ToList();
        }

        private static void CheckIfToCountOrAdd(List<UniqueLetter> uniqueLetters, string letter, string firstPart, string secondPart)
        {
            if (!uniqueLetters.Any(uniqueLetter => uniqueLetter.Character.Equals(letter)))
            {
                uniqueLetters.Add(new UniqueLetter
                {
                    Character = letter,
                    Count = 1,
                    FirstPart = firstPart,
                    SecondPart = secondPart
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
                    CheckIfToCountOrAdd(uniqueLetters, letter, letter, "");
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
            var data = "In 50 F.E. (\"Foundation Era\"), the Encyclopedia Foundation, tasked with preserving the Empire's knowledge, is established on the mineral-poor agricultural planet Terminus, and occupies the planet's single large settlement, Terminus City. The city's affairs are managed by its first Mayor, Salvor Hardin, under the authority of the Board, hidebound scholars whose main concern is publishing the Encyclopedia. Hardin believes Terminus is in danger of conquest by the four neighboring prefectures of the Empire, the strongest of which is Anacreon. When the Board resists Hardin's efforts against the threat, he and his chief advisor, Yohan Lee, seize power. Hardin then visits the three weaker kingdoms and convinces them that they must unite to prevent the Foundation's nuclear technology from falling into the hands of Anacreon alone. The three issue a joint ultimatum that all be allowed to receive nuclear power from Terminus, making it indispensable to all and protected by a delicate balance of power. A vault containing Seldon's recorded messages opens, and reveals that he had planned this whole course of events by means of psychohistory, and that the Foundation is destined to grow into a new galactic empire.";
            var corpus = data.Split(' ').ToList();
            return corpus;
        }
    }

    public class UniqueLetter
    {
        public string Character { get; set; }
        public int Count { get; set; }
        public string? FirstPart { get; set; }
        public string? SecondPart { get; set; }
    }
}