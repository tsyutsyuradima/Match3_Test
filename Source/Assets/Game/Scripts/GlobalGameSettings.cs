using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GlobalGameSettings
    {
        GlobalGameSettings()
        {
            col = 3;
            row = 3;
            score = 0;
        }

        // количество елементов в одной колонке
        int col;
        public int Col { get { return col; } set { } }

        // количество елементов в одном радке
        int row;
        public int Row { get { return row; } set { } }


        int score;
        public int Score { get { return score; } set { score = value; } }

        public void SetGameGridSize(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        static GlobalGameSettings instance;
        public static GlobalGameSettings Instance
        {
            get {   if (instance == null)
                        instance = new GlobalGameSettings();
                    return instance;
                }
            set { }
        }
    }
}