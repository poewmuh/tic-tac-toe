using UnityEngine;

namespace TicTacToe.Gameplay.Helper
{
    public static class TicTacHelper
    {
        private static readonly int[][] _winnerLine = {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
            new[]{0,4,8}, new[]{2,4,6}
        };

        private static bool IsHaveWinner()
        {
            return false;
        }
    }
}