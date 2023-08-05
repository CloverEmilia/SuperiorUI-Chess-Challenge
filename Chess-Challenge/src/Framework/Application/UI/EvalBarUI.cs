﻿using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.CodeDom;

namespace ChessChallenge.Application
{
    public static class EvalBarUI
    {
        static int evalBarPositionX = 480;
        static int evalBarPositionY = 100;
        static int evalBarSizeX = 40;
        static int evalBarSizeY = 847; //I hate this number
        public static void DrawEvalBar(ChallengeController controller)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            //place a black rectangle over the entire area and then cover with white what's actually needed
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), UIHelper.ScaleInt(evalBarSizeY), BoardUI.theme.weakNeutralTextColor);
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), UIHelper.ScaleInt(evalBarSizeY) /((Raylib.GetMouseY() / 10) + 1), BoardUI.theme.strongNeutralTextColor);
        }

        public static void GetStockfishEval (ChallengeController controller)
        {
            //for now stockfish just goes here, but we can break off if I want to add stockfishbot by default

        }
    }
}