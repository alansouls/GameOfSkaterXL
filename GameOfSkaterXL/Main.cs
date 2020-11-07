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
            GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Press F6 to pass turn to next player (also unsets current trick)");
            currentTrickY += currentTrickOfssetY;
            if (!Main.GameOfSkateManagerInstance.IsSettingTrick && !Main.GameOfSkateManagerInstance.IsCopyingTrick)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Press F1 to set or copy a trick");
                currentTrickY += currentTrickOfssetY;
            }
            if (Main.GameOfSkateManagerInstance.IsTrickSet)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Press F5 to unset the current trick");
                currentTrickY += currentTrickOfssetY;
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
                if (Main.GameOfSkateManagerInstance.IsLastChance)
                {
                    GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Last chance to land current trick!");
                    currentTrickY += currentTrickOfssetY;
                }
            }
            if (Main.GameOfSkateManagerInstance.WasTrickRepeated)
            {
                GUI.Label(new Rect(currentTrickX, currentTrickY, maxPhraseWidht, 30), "Couldn't set repeated trick");
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

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
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

            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameOfSkateManagerInstance.PrepareSetOrCopyTrick();
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                GameOfSkateManagerInstance.UnsetCurrentTrick();
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                GameOfSkateManagerInstance.UnsetCurrentTrick();
                GameOfSkateManagerInstance.NextPlayer();
            }
        }

        static void OnComboEnded(TrickCombo trickCombo)
        {
            if (GameOfSkateManagerInstance.IsCopyingTrick)
                GameOfSkateManagerInstance.VerifiyTrickCopied(trickCombo);
            else if (GameOfSkateManagerInstance.IsSettingTrick)
                GameOfSkateManagerInstance.VerifyTrickSet(trickCombo);
        }
    }
}
