using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnagramServer
{
    public static class AnagramService
    {
        public static string CountAnagrams(string path, string targetWord)
        {
            string text = File.ReadAllText(path);

            string[] words = Regex
                .Split(text, @"\W+")
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToArray();

            string normalizedTarget = Normalize(targetWord);

            int count = 0;

            foreach (string word in words)
            {
                if (
                    Normalize(word) == normalizedTarget
                    &&
                    word.ToLower() != targetWord.ToLower()
                )
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return "Nema anagrama.";
            }

            return $"Broj anagrama za rec '{targetWord}' je: {count}";
        }

        private static string Normalize(string word)
        {
            char[] chars =
                word.ToLower().ToCharArray();

            Array.Sort(chars);

            return new string(chars);
        }
    }
}