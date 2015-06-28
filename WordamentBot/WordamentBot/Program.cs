using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordamentBot
{
    class Program
    {
        static Dictionary<string, bool> wordList = new Dictionary<string, bool>();
        static Dictionary<string, List<int>> validWords = new Dictionary<string, List<int>>();
        static string[,] game = new string[,]{
            {"I","S","E","R"},
            {"N","S","U","F"},
            {"A","E","P","O"},
            {"P","H","R","U"}
        };

        static int baseX = 225;
        static int baseY = 220;
        static int incrementX = 150;
        static int incrementY = 150;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            //  mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private static void InitPixels()
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            double multiplierX = resolution.Size.Width / 1600.0;
            double multiplierY = resolution.Size.Height / 900.0;
            baseX = (int)(multiplierX * baseX);
            baseY = (int)(multiplierY * baseY);
            incrementX = (int)(incrementX * multiplierX);
            incrementY = (int)(incrementY * multiplierY);
        }
        private static void LoadDictionary()
        {
            File.ReadAllLines(@"..\..\Files\Words.txt").Distinct().Where(t => t.Length >= 3).ToList().ForEach(word => wordList.Add(word.ToUpper(), true));
        }
        static bool IsValidPosition(int pos)
        {
            return (pos >= 0 && pos < 4);
        }
        static void WordamentCrack(string currentString, int currentPosition, List<int> coveredLetters)
        {
            if (String.IsNullOrEmpty(currentString))
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        WordamentCrack(game[i, j], i * 10 + j, new List<int>() { i * 10 + j });
                    }
                }
            }
            else
            {
                int row = currentPosition / 10;
                int col = currentPosition % 10;
                if (wordList.ContainsKey(currentString) && !validWords.ContainsKey(currentString))
                {
                    validWords.Add(currentString, coveredLetters);
                }
                for (int r = row - 1; r <= row + 1; r++)
                {
                    for (int c = col - 1; c <= col + 1; c++)
                    {
                        if (r == row && c == col)
                        {
                            continue;
                        }
                        int posToStore = r * 10 + c;
                        if (IsValidPosition(r) && IsValidPosition(c) && !coveredLetters.Contains(posToStore))
                        {
                            List<int> covLetters = new List<int>();
                            coveredLetters.ForEach(t => covLetters.Add(t));
                            covLetters.Add(posToStore);
                            WordamentCrack(currentString + game[r, c], posToStore, covLetters);
                        }
                    }
                }
            }
        }
        static void BotInAction()
        {
            foreach (KeyValuePair<string, List<int>> keyValue in validWords)
            {
                int x = 0, y = 0;
                for(int pos=0;pos<keyValue.Value.Count;pos++)
                {
                    y = keyValue.Value[pos] / 10;
                    x = keyValue.Value[pos] % 10;
                    LeftMouseClick(baseX + incrementX * x, baseY + incrementY * y);
                    Thread.Sleep(1);
                }
                mouse_event(MOUSEEVENTF_LEFTUP, 225 + 150 * x, 220 + 150 * y, 0, 0);
                Thread.Sleep(1);
            }
        }
        static void Main(string[] args)
        {
            InitPixels();
            //  Thread.Sleep(3000);
            //  LeftMouseClick(225 , 220);
            //  LeftMouseClick(375, 220);
            //  LeftMouseClick(525, 370);
            ////  LeftMouseClick(375, 220);
            //  Thread.Sleep(100000);
            LoadDictionary();
            WordamentCrack(String.Empty, 0, null);
            BotInAction();
        }
    }
}
