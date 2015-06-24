using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordamentBot
{
    class Program
    {
        static Dictionary<string, bool> wordList = new Dictionary<string, bool>();
        static List<string> validWords=new List<string>();
        static string[,] game = new string[,]{
            {"E","S","N","I"},
            {"O","L","T","E"},
            {"Z","E","A","S"},
            {"N","D","E","N"}
        };
        private static void LoadDictionary()
        {
            File.ReadAllLines(@"..\..\Files\Words.txt").Distinct().Where(t => t.Length >= 3).ToList().ForEach(word => wordList.Add(word.ToUpper(), true));
        } 
        static bool IsValidPosition(int pos)
        {
            return (pos>=0 && pos<4);
        }
        static void WordamentCrack(string currentString, int currentPosition, List<int> coveredLetters)
        {
            int row = currentPosition / 10;
            int col = currentPosition % 10;
            if(wordList.ContainsKey(currentString))
            {
                validWords.Add(currentString);
            }
            for(int r=row-1;r<=row+1;r++)
            {
                for(int c=col-1;c<=col+1;c++)
                {
                    if(r==row && c==col)
                    {
                        continue;
                    }
                    int posToStore=r*10+c;
                    if(IsValidPosition(r) && IsValidPosition(c) && !coveredLetters.Contains(posToStore))
                    {
                        List<int> covLetters=new List<int>();
                        coveredLetters.ForEach(t => covLetters.Add(t));
                        covLetters.Add(posToStore);
                        WordamentCrack(currentString + game[r, c], posToStore, covLetters);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            LoadDictionary();
            for (int i = 0; i < 4;i++ )
            {
                for(int j=0;j<4;j++)
                {
                    WordamentCrack(game[i,j], i * 10 + j, new List<int>() { i * 10 + j });
                }
            }
            validWords = validWords.Distinct().OrderByDescending(t=>t.Length).ToList();
            Console.Read();
        }
    }
}
