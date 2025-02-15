﻿using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using ChessChallenge.Chess;
using System.Text.RegularExpressions;
using static ChessChallenge.Application.ChallengeController;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using ChessChallenge.Application;

namespace ChessChallenge.Application
{
    public static class MenuUI
    {
        //Static Variable declaration
        //PlayerType Variables
        static readonly ChallengeController.PlayerType[] playerArray = (ChallengeController.PlayerType[])Enum.GetValues(typeof(ChallengeController.PlayerType)); //array of all players registered in ChallengeController
        static ChallengeController.PlayerType selectedBotA;
        static ChallengeController.PlayerType selectedBotB;
        static bool selectingBotBAndNotA; // Variable controlling which index is being written to
                                          //Selection Tabs Variables
        static readonly int botsPerTab = 12; // Number of bots per tab
        static int currentTab = 0; // Current tab index
                                   //Screen Size Variables
        static readonly float amountToChangeScreenByPerButtonPress = 1.2f;
        static float screenSizeMultiplier = 1;
            //Game Saving Variables
        public static readonly string customSaveFilePath = @"C:\replace this with your desired directory, goes to documents by default";
        static bool isIncrimentalGameSaveCurrentlyOn;

        public static void DrawButtons(ChallengeController controller)
        {
            //Initialize Variables for each individual button (not static because ref is needed, may as well keep the derivitives in the same context)
            Vector2 buttonPos = UIHelper.Scale(new Vector2(260, 110));
            Vector2 buttonSize = UIHelper.Scale(new Vector2(260, 35));
            float spacing = buttonSize.Y * 1.1f;
            float breakSpacing = spacing * 0.6f;

            //Bot Selection variable declaration
            Vector2 selectionTextStartingPosition = new(buttonPos.X * .5f, buttonPos.Y * .6f);
            Vector2 playerTwoOffsetPosition = Vector2.UnitY * buttonPos.Y * -0.3f; //I should make this normal-er
            int selectionTextSize = UIHelper.ScaleInt(32);
            int selectionTextSpacing = UIHelper.ScaleInt(1);

            //Draw the UI for bot selection
            UIHelper.DrawText("Player 1: " + selectedBotA, selectionTextStartingPosition, selectionTextSize, selectionTextSpacing, selectingBotBAndNotA ? BoardUI.theme.weakNeutralTextColor : BoardUI.theme.positiveTextColor);
            UIHelper.DrawText("Player 2: " + selectedBotB, selectionTextStartingPosition + playerTwoOffsetPosition, selectionTextSize, selectionTextSpacing, !selectingBotBAndNotA ? BoardUI.theme.weakNeutralTextColor : BoardUI.theme.positiveTextColor);
            //and the tabs
            UIHelper.DrawText("tab:", buttonPos * 1.55f + Vector2.UnitY * buttonPos.Y * -0.3f, selectionTextSize, selectionTextSpacing, BoardUI.theme.weakNeutralTextColor); //gods help me this allignment is painful, it doesn't matter for now
            UIHelper.DrawText((currentTab + 1) + "/" + ((int)(playerArray.Length / botsPerTab) + 1), buttonPos * 1.55f + Vector2.UnitY * buttonPos.Y * -0.05f, selectionTextSize, selectionTextSpacing, BoardUI.theme.weakNeutralTextColor); //gods help me this allignment is painful, it doesn't matter for now


            //Tab management (this might be garbage)
            // keep current tab within bounds
            if (currentTab < 0)
            {
                currentTab = playerArray.Length / botsPerTab;
            }
            else if (currentTab > (playerArray.Length / botsPerTab))
            {
                currentTab = 0;
            }
            //make sure not exactly equal
            if (currentTab < 0 || currentTab >= Math.Ceiling((float)playerArray.Length / botsPerTab))
            {
                currentTab = 0;
            }


            //for every bot
            for (int i = 0; i < playerArray.Length; i++)
            {
                //If should be rendered within the current tab
                if ((botsPerTab * currentTab) - 1 < i && i < (botsPerTab * currentTab) + botsPerTab)
                {
                    //Create bot button
                    if (NextButtonInRow(playerArray.GetValue(i).ToString(), ref buttonPos, spacing, buttonSize))
                    {
                        //(write and switch which bot being selected when button pressed)
                        if (selectingBotBAndNotA == false)
                        {
                            selectedBotA = (ChallengeController.PlayerType)playerArray.GetValue(i);
                        }
                        else
                        {
                            selectedBotB = (ChallengeController.PlayerType)playerArray.GetValue(i);
                        }

                        selectingBotBAndNotA = !selectingBotBAndNotA;
                    }

                    //if on the last bot
                    if (i == playerArray.Length)
                    {
                        //if needed,
                        int remainingEmptyButtons = playerArray.Length % botsPerTab;
                        //fill the remaining slots with empty buttons
                        for (int j = 0; j < remainingEmptyButtons; j++)
                        {
                            if (NextButtonInRow("[end of list]", ref buttonPos, spacing, buttonSize))
                            {

                            }
                        }
                    }
                }
            }

            //Index variables (might be possible to derive elsewhere but it's fine here)
            int startIndex = currentTab * botsPerTab;
            int endIndex = Math.Min(startIndex + botsPerTab, playerArray.Length);

            //Fills up the remaining slots with empty buttons
            if (endIndex < startIndex + botsPerTab && playerArray.Length > botsPerTab)
            {
                int remainingEmptyButtons = startIndex + botsPerTab - endIndex;
                for (int i = 0; i < remainingEmptyButtons; i++)
                {
                    if (NextButtonInRow("[end of list]", ref buttonPos, spacing, buttonSize))
                    {
                        // You can customize the behavior of the dummy buttons here if needed.
                    }
                }
            }

            //Tab change buttons
            if (NextButtonInRow("<- last page", ref buttonPos, spacing * .9f, buttonSize * .7f))
            {
                currentTab--;
            }
            if (NextButtonInRow("next page ->", ref buttonPos, spacing * 1.2f, buttonSize * .7f))
            {
                currentTab++;
            }


            if (NextButtonInRow("START MATCH", ref buttonPos, spacing * 1.25f, buttonSize * 1.25f))
            {
                //if one of the players is a human
                if (selectedBotA == ChallengeController.PlayerType.Human || selectedBotB == ChallengeController.PlayerType.Human)
                {
                    //Switch the bot teams, so the player starts on the other side
                    selectedBotA ^= selectedBotB;
                    selectedBotB = selectedBotA ^ selectedBotB;
                    selectedBotA ^= selectedBotB;
                }

                //Begin the game
                controller.StartNewBotMatch(selectedBotA, selectedBotB);
            }

            if (NextButtonInRow("Start Tournament", ref buttonPos, spacing, buttonSize))
            {
                controller.isTournamentActive = true;
                controller.StartNewBotMatch((PlayerType)1, (PlayerType)1);
            }

            // Resources/External buttons
            buttonPos.Y += breakSpacing;
            if (NextButtonInRow("rules", ref buttonPos, spacing, buttonSize * .7f))
            {
                FileHelper.OpenUrl("https://github.com/SebLague/Chess-Challenge");
            }
            if (NextButtonInRow("api", ref buttonPos, spacing, buttonSize * .7f))
            {
                FileHelper.OpenUrl("https://seblague.github.io/chess-coding-challenge/documentation/");
            }


            // functional buttons
            buttonPos.Y += breakSpacing;

            if(controller.fastForward == true){
                if (NextButtonInRow("Fast Forward [ON]", ref buttonPos, spacing, buttonSize))
                {
                    controller.fastForward = false;
                    Settings.RunBotsOnSeparateThread = Settings.RunBotsOnSeparateThread = true;
                }
            }
            else
            {
                if (NextButtonInRow("Fast Forward off", ref buttonPos, spacing, buttonSize))
                {
                    controller.fastForward = true;
                    Settings.RunBotsOnSeparateThread = Settings.RunBotsOnSeparateThread = false;
                }
            }

            if (isIncrimentalGameSaveCurrentlyOn == true)
            {
                if (NextButtonInRow("save pgns [ON]", ref buttonPos, spacing, buttonSize))
                {
                    isIncrimentalGameSaveCurrentlyOn = false;
                }
            }
            else
            {
                if (NextButtonInRow("save pgns off", ref buttonPos, spacing, buttonSize))
                {
                    isIncrimentalGameSaveCurrentlyOn = true;
                }
            }

            if (NextButtonInRow("--- screen", ref buttonPos, spacing * .9f, buttonSize * .7f))
            {
                screenSizeMultiplier /= amountToChangeScreenByPerButtonPress;
                UpdateApplicationWindowSize();
            }
            if (NextButtonInRow("screen +++", ref buttonPos, spacing * 1.2f, buttonSize * .7f))
            {
                screenSizeMultiplier *= amountToChangeScreenByPerButtonPress;
                UpdateApplicationWindowSize();
            }

            static void UpdateApplicationWindowSize()
            {
                Program.SetWindowSize(new Vector2(Settings.defaultScreenX, Settings.defaultScreenY) * screenSizeMultiplier);
            }
            if (NextButtonInRow("Exit (ESC)", ref buttonPos, spacing, buttonSize))
            {
                Environment.Exit(0);
            }

            static bool NextButtonInRow(string name, ref Vector2 pos, float spacingY, Vector2 size)
            {
                bool pressed = UIHelper.Button(name, pos, size);
                pos.Y += spacingY;
                return pressed;
            }
        }
    }
}