
using System.Collections.Generic;
using System.Linq;

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

        public List<string> TricksDone;

        public bool WasTrickRepeated;

        public GameOfSkateManager()
        {
            Reset();
        }

        public void Reset()
        {
            WasTrickRepeated = false;
            IsTrickSet = false;
            CurrentTrick = "";
            IsSettingTrick = false;
            IsCopyingTrick = false;
            PlayerLetters = new int[2];
            PlayerCount = 2;
            for (int i = 0; i < PlayerCount; ++i)
                PlayerLetters[i] = 0;
            CurrentPlayerTurn = 0;
            IsTrickCopied = false;
            GameWord = "SKATE";
            TricksDone = new List<string>();
        }

        /// <summary>
        /// Returns if game is ended
        /// </summary>
        /// <param name="trickCombo"></param>
        /// <returns></returns>
        public void VerifiyTrickCopied(TrickCombo trickCombo)
        {
            IsCopyingTrick = false;
            IsTrickSet = false;
            var copiedTrick = string.Join(" ", trickCombo.Tricks.Select(s => s.ToString()));
            if (copiedTrick == CurrentTrick && trickCombo.Landed)
            {
                IsTrickCopied = true;
            }
            else
            {
                PlayerLetters[CurrentPlayerTurn] += 1;
                if (PlayerLetters[CurrentPlayerTurn] >= GameWord.Length)
                {
                    Reset();
                    return;
                }
                IsTrickCopied = false;
            }
            CurrentTrick = "";
            NextPlayer();
        }

        public void VerifyTrickSet(TrickCombo trickCombo)
        {
            IsSettingTrick = false;
            var trickName = string.Join(" ", trickCombo.Tricks.Select(s => s.ToString()));
            var wasTrickRepeated = TricksDone.Contains(trickName);
            if (trickCombo.Landed && !wasTrickRepeated)
            {
                IsTrickSet = true;
                CurrentTrick = trickName;
                TricksDone.Add(CurrentTrick);
            }
            WasTrickRepeated = wasTrickRepeated && trickCombo.Landed;
            NextPlayer();
        }

        public void NextPlayer()
        {
            var nextPlayer = CurrentPlayerTurn + 1;
            CurrentPlayerTurn = nextPlayer >= PlayerCount ? 0 : nextPlayer;
        }

        public void UnsetCurrentTrick()
        {
            if (!IsTrickSet)
                return;
            IsTrickSet = false;
            IsSettingTrick = false;
            IsCopyingTrick = false;
            TricksDone.RemoveAt(TricksDone.Count - 1);
        }

        public void PrepareSetOrCopyTrick()
        {
            if (IsSettingTrick || IsCopyingTrick)
                return;
            if (IsTrickSet)
            {
                WasTrickRepeated = false;
                IsCopyingTrick = true;
            }
            else
            {
                IsSettingTrick = true;
            }
        }
    }
}
