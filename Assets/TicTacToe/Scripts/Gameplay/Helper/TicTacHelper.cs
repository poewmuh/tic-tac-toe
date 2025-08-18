using System;
using TicTacToe.Gameplay.Core;
using Unity.Netcode;
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

        public static bool IsHaveWinner(NetworkList<CellValue> board, out int[] winLine)
        {
            winLine = Array.Empty<int>();
            foreach (var line in _winnerLine)
            {
                var boardMark = board[line[0]].Value;
                if (boardMark != Cell.Empty && boardMark == board[line[1]].Value && boardMark == board[line[2]].Value)
                {
                    winLine = line;
                    return true;
                }
            }

            return false;
        }

        public static bool IsBoardFull(NetworkList<CellValue> board)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i].Value is Cell.Empty) return false;
            }
            
            return true;
        }
    }
}