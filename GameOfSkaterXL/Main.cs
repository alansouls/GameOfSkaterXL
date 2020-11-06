using System;
using UnityEngine;
using UnityModManagerNet;
using System.Collections;
using System.Linq;
using RootMotion.FinalIK;
using System.Runtime.Remoting.Messaging;
using Rewired.UI.ControlMapper;

namespace GameOfSkaterXL
{

    public class GameSkateUI : MonoBehaviour
    {
        public void OnGUI()
        {
            var maxPhraseWidht = 500;
            var scoreBoardY = 25;
            var scoreBoardOffsetY = 25;
            int scoreBoardX = Screen.width / 2 - maxPhraseWidht;
            GUI.Label(new Rect(scoreBoardX, scoreBoardY, maxPhraseWidht, 30), "GAME OF SKATE SCOREBOARD");
            scoreBoardY += scoreBoardOffsetY;
            for (int i = 0; i < Main.GameOfSkateManagerInstance.PlayerCount; ++i)
            {
                char currentPlayerChar = i == Main.GameOfSkateManagerInstance.CurrentPlayerTurn ? '*' : '\0';
                GUI.Label(new Rect(scoreBoardX, scoreBoardY, maxPhraseWidht, 30), $"{currentPlayerChar} Player {i + 1}: {Main.GameOfSkateManagerInstance.GameWord.Substring(0, Main.GameOfSkateManagerInstance.PlayerLetters[i])}");
                scoreBoardY += scoreBoardOffsetY;
            }

            var currentTrickY = 25;
            var currentTrickOfssetY = 25;
            int currentTrickX = Screen.width / 2;
            if (!Main.GameOfSkateManagerInstance.IsSettingTrick && !Main.GameOfSkateManagerInstance.IsCopyingTrick)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Press F1 to set or copy a trick");
                currentTrickY += currentTrickOfssetY;
            }
            if (Main.GameOfSkateManagerInstance.IsTrickSet)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), $"Current Trick: {Main.GameOfSkateManagerInstance.CurrentTrick}");
                currentTrickY += currentTrickOfssetY;
            }
            else if (Main.GameOfSkateManagerInstance.IsSettingTrick)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Waiting for trick to be set");
                currentTrickY += currentTrickOfssetY;
            }
            if (Main.GameOfSkateManagerInstance.IsCopyingTrick)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Waiting for trick to be copied");
                currentTrickY += currentTrickOfssetY;
            }
        }
    }

    public static class Main
    {

        public static GameSkateUI gameUI;

        public static GameOfSkateManager GameOfSkateManagerInstance;
        static void Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* active or inactive */)
        {
            if (value && GameOfSkateManagerInstance == null)
            {
                modEntry.Logger.Log("Created Instance");
                GameOfSkateManagerInstance = new GameOfSkateManager();
                TrickManager.Instance.onComboEnded += OnComboEnded;
                gameUI = new GameObject().AddComponent<GameSkateUI>();
                GameObject.DontDestroyOnLoad(gameUI.gameObject);
            }
            else
            {
                modEntry.Logger.Log("Removed Instance");
                GameOfSkateManagerInstance = null;
                GameObject.Destroy(gameUI.gameObject);
            }

            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (GameOfSkateManagerInstance == null)
                return;

            var canSet = !GameOfSkateManagerInstance.IsSettingTrick && !GameOfSkateManagerInstance.IsCopyingTrick;
            if (Input.GetKeyDown(KeyCode.F1) && canSet)
            {
                if (GameOfSkateManagerInstance.IsTrickSet)
                {
                    GameOfSkateManagerInstance.IsCopyingTrick = true;
                }
                else
                {
                    GameOfSkateManagerInstance.IsSettingTrick = true;
                }
            }
        }

        static void OnComboEnded(TrickCombo trickCombo)
        {
            if (GameOfSkateManagerInstance.IsCopyingTrick)
            {
                //TODO Implement UI feedback for copying tricks
                GameOfSkateManagerInstance.IsCopyingTrick = false;
                GameOfSkateManagerInstance.IsTrickSet = false;
                var copiedTrick = string.Join(" ", trickCombo.Tricks.Select(s => s.ToString()));
                if (copiedTrick == GameOfSkateManagerInstance.CurrentTrick && trickCombo.Landed)
                {
                    GameOfSkateManagerInstance.IsTrickCopied = true;
                }
                else
                {
                    GameOfSkateManagerInstance.PlayerLetters[GameOfSkateManagerInstance.CurrentPlayerTurn] += 1;
                    if (GameOfSkateManagerInstance.PlayerLetters[GameOfSkateManagerInstance.CurrentPlayerTurn] > GameOfSkateManagerInstance.GameWord.Length)
                    {
                        GameOfSkateManagerInstance = new GameOfSkateManager();
                        return;
                    }
                    GameOfSkateManagerInstance.IsTrickCopied = false;
                }
                GameOfSkateManagerInstance.CurrentTrick = "";
                GameOfSkateManagerInstance.NextPlayer();
            }
            else if (GameOfSkateManagerInstance.IsSettingTrick)
            {
                //TODO Implement UI feedback for setting tricks
                GameOfSkateManagerInstance.IsSettingTrick = false;
                if (trickCombo.Landed)
                {
                    GameOfSkateManagerInstance.IsTrickSet = true;
                    GameOfSkateManagerInstance.CurrentTrick = string.Join(" ", trickCombo.Tricks.Select(s => s.ToString()));
                }
                GameOfSkateManagerInstance.NextPlayer();
            }
        }
    }
}
