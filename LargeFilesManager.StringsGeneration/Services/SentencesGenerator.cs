using LargeFilesManager.StringsGeneration.Interfaces;

namespace LargeFilesManager.StringsGeneration.Services
{
    public class SentencesGenerator : ISentencesGenerator
    {
        public List<string> Generate(int numberOfSentences)
        {
            var subjects = new[]
            {
                "Cat", "Dog", "Friend", "Artist", "Scientist", "Musician", "Teacher", "Child",
                "Runner", "Chef", "Bird", "Traveler", "Photographer", "Magician", "Gardener",
                "Engineer", "Lion", "Dolphin", "Astronaut", "Painter"
            };

            var verbs = new[]
            {
                "jumps over", "runs towards", "paints", "writes", "dances on", "sings about", "dreams of", "thinks about",
                "walks into", "gazes at", "builds", "flies above", "dives into", "laughs at", "waves at", "searches for",
                "creates", "fixes", "plays with", "discovers"
            };

            var objects = new[]
            {
                "a fence.", "the park.", "a masterpiece.", "a novel.", "the stars.", "a mysterious island.", "the sea.",
                "a hidden treasure.", "the mountain top.", "a beautiful garden.", "a broken clock.", "a flying kite.",
                "the sandy beach.", "an old photograph.", "a golden key.", "the deep forest.", "an ancient castle.",
                "the bustling city.", "a magical spell.", "the bright sunrise."
            };

            var random = new Random();
            var sentences = new List<string>();

            for (var i = 0; i < numberOfSentences; i++)
            {
                var sentence = $"{subjects[random.Next(subjects.Length)]} {verbs[random.Next(verbs.Length)]} {objects[random.Next(objects.Length)]}";
                sentences.Add(sentence);
            }

            return sentences;
        }
    }
}
