
using System.Collections.Generic;

namespace GameOfSkaterXL
{
    public class GameOfSkateManager
    {
        public bool IsTrickSet;

        public string CurrentTrick;

        public bool IsSettingTrick;

        public bool IsCopyingTrick;

        public int[] PlayerLetters;

        public string GameWord;

        public int PlayerCount;

        public int CurrentPlayerTurn;

        public bool IsTrickCopied;

        public GameOfSkateManager()
        {
            IsTrickSet = false;
            CurrentTrick = "";
            IsSettingTrick = false;
            IsCopyingTrick = false;
            PlayerLetters = new int[2];
            PlayerLetters[0] = 0;
            PlayerLetters[1] = 0;
            PlayerCount = 2;
            CurrentPlayerTurn = 0;
            IsTrickCopied = false;
            GameWord = "SKATE";
        }

        public void NextPlayer()
        {
            var nextPlayer = CurrentPlayerTurn + 1;
            CurrentPlayerTurn = nextPlayer >= PlayerCount ? 0 : nextPlayer;
        }
    }
}
