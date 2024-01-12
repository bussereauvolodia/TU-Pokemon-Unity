using System;
using UnityEngine;

namespace _2023_GC_A2_Partiel_POO.Level_2
{
    public class Level
    {
        public int RequiredXP { get; private set; }
        public int LevelAmount { get; private set; }
        public int CurrentXP { get; private set; }

        public Level(int requiredXP)
        {
            LevelAmount = 1;
            RequiredXP = requiredXP;
            CurrentXP = 0;
        }

        public void AddXP(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException();
            CurrentXP += amount;
            while (CurrentXP >= RequiredXP)
            {
                CurrentXP -= RequiredXP;
                RequiredXP *= 2;
                LevelAmount++;
            }
        }

    }
}
