using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

class AI
{
    Dictionary<int, int> difficultyMapping = new()
    {
        { 0, 1 },
        { 1, 3 },
        { 2, 9 },
    };

    TicTacToe ticTacToe;
    int turn;
    bool flipped;

    public Action<IEnumerator> OnComputeAI;
    public Action<IEnumerator> OnReceiveTurn;

    public AI(TicTacToe ticTacToe, int turn)
    {
        this.ticTacToe = ticTacToe;
        this.turn = turn;

        this.ticTacToe.onTurnChanged += (currentTurn) =>
        {
            if (currentTurn != this.turn) return;
            ComputeAI();
        };
    }

    public void Reset()
    {
        flipped = Data.selectedMode == ModeSelect.ModeType.FlipTacToe;
        UnityEngine.Debug.Log(flipped);
        UnityEngine.Debug.Log(Data.difficultyLevel);

        if (turn == ticTacToe.getTurn)
        {
            ComputeAI();
        }
    }

    void ComputeAI()
    {
        OnComputeAI?.Invoke(Compute());
    }

    IEnumerator Compute()
    {
        yield return new WaitForSeconds(1f);

        float eval = flipped ? float.MinValue : float.MaxValue;
        int bestMove = -1;

        foreach (var move in ticTacToe.getAvailableMoves)
        {
            var testBoard = ticTacToe.Clone().MakeMove(move / 3, move % 3);
            float score = MiniMax(testBoard, true, float.MinValue, float.MaxValue, difficultyMapping[Data.difficultyLevel]);

            if (flipped)
            {
                if (score > eval)
                {
                    eval = score;
                    bestMove = move;
                }
            }
            else
            {
                if (score < eval)
                {
                    eval = score;
                    bestMove = move;
                }
            }
        }

        ticTacToe.MakeMove(bestMove / 3, bestMove % 3);
    }

    float MiniMax(TicTacToe board, bool isMaximizing, float alpha, float beta, int depth)
    {
        if (depth == 0 || board.GameOver())
        {
            return board.Score() * (float)(depth + 1);
        }

        if (isMaximizing)
        {
            float maxEval = float.MinValue;
            foreach (var move in board.getAvailableMoves)
            {
                var testBoard = board.Clone().MakeMove(move / 3, move % 3);

                float eval = MiniMax(testBoard, !isMaximizing, alpha, beta, depth - 1);

                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha) break;
            }

            return maxEval;
        }
        else
        {
            float minEval = float.MaxValue;

            foreach (var move in board.getAvailableMoves)
            {
                var testBoard = board.Clone().MakeMove(move / 3, move % 3);

                float eval = MiniMax(testBoard, !isMaximizing, alpha, beta, depth - 1);

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha) break;
            }

            return minEval;
        }
    }
}