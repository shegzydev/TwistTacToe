using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

class TicTacToe
{
    public event Action<string> Log = new((msg) => { });
    public event Action<(int x_board, int o_board)> onPlayed;
    public event Action<int> onPop;
    public event Action<int> onTurnChanged;
    public event Action<(int score, int winOrientation)> onGameEnd;

    public event Action<int> onStartPopTimer;
    public event Action<Action> onReverseBoard;

    const int boardSize = 9;
    int currentTurn;

    bool gameOver;
    int finalScore;

    System.Random rng = new System.Random();

    //x_board : o_board
    int[] board = { 0b000000000, 0b000000000 };
    Queue<int>[] playHistory = { new Queue<int>(), new Queue<int>() };

    int FILLED_MASK = 0b111111111;
    int[] WIN_MASKS = {
        0b000000111,
        0b000111000,
        0b111000000,
        0b001001001,
        0b010010010,
        0b100100100,
        0b100010001,
        0b001010100
    };

    public void Reset()
    {
        currentTurn = rng.Next(2);

        gameOver = false;
        finalScore = 0;

        board[0] = 0;
        board[1] = 0;

        playHistory[0].Clear();
        playHistory[1].Clear();
    }

    public TicTacToe MakeMove(int row, int col)
    {
        int linear = row * 3 + col;
        if (!CellFree(linear))
        {
            throw new ArgumentException("⚠️ Cell filled, not allowed");
        }

        board[currentTurn] |= (1 << linear);

        playHistory[currentTurn].Enqueue(linear);

        TryThreeTacToeTwist();

        onPlayed?.Invoke((board[0], board[1]));

        if (CheckWin(board[currentTurn], out int winOrientation))
        {
            finalScore = ((currentTurn == 0) ? 1 : -1);

            gameOver = true;

            onGameEnd?.Invoke((finalScore, winOrientation));
        }
        else if (CheckDraw())
        {
            finalScore = 0;

            gameOver = true;

            onGameEnd?.Invoke((finalScore, -1));
        }
        else
        {
            NextTurn();
        }

        TryCursedTwist();

        return this;
    }

    private void TryThreeTacToeTwist()
    {
        if (Data.selectedMode != ModeSelect.ModeType.ThreeTacToe) return;

        if (playHistory[currentTurn].Count <= 3) return;

        PopOldest();
    }

    private void TryTickTacToeTwist()
    {
        if (Data.selectedMode != ModeSelect.ModeType.TickTacToe) return;

        if (playHistory[currentTurn].Count <= 0) return;

        onStartPopTimer?.Invoke(currentTurn);
    }

    private void TryCursedTwist()
    {
        if (Data.selectedMode != ModeSelect.ModeType.Cursed || gameOver) return;

        for (int i = 0; i < board.Length; i++)
        {
            board[i] = ReverseBitMask(board[i]);
        }

        onReverseBoard?.Invoke(() => onPlayed?.Invoke((board[0], board[1])));
    }

    int ReverseBitMask(int mask)
    {
        int result = 0;
        for (int i = 0; i < boardSize; i++)
        {
            result <<= 1;
            result |= (mask & 1);
            mask >>= 1;
        }
        return result;
    }

    public bool CellFree(int cellIndex)
    {
        int occupiedCells = board[0] | board[1];
        return (occupiedCells & (1 << cellIndex)) == 0;
    }

    void NextTurn()
    {
        currentTurn++;
        currentTurn %= 2;

        onTurnChanged?.Invoke(currentTurn);

        TryTickTacToeTwist();
    }

    public bool CheckWin(int board, out int winOrientation)
    {
        winOrientation = 0;
        foreach (var mask in WIN_MASKS)
        {
            if ((board & mask) == mask)
            {
                return true;
            }
            winOrientation++;
        }
        return false;
    }

    public bool CheckDraw()
    {
        int occupiedCells = board[0] | board[1];
        return (occupiedCells & FILLED_MASK) == FILLED_MASK;
    }

    public TicTacToe Clone()
    {
        return new TicTacToe
        {
            currentTurn = currentTurn,
            board = new int[] { board[0], board[1] },
            playHistory = new Queue<int>[] { new(playHistory[0].ToArray()), new(playHistory[1].ToArray()) },
            gameOver = gameOver,
            finalScore = finalScore,
        };
    }

    public int Score()
    {
        return gameOver ? finalScore : 0;
    }

    public bool GameOver()
    {
        return gameOver;
    }

    public int[] getAvailableMoves
    {
        get
        {
            List<int> availableMoves = new();
            for (int i = 0; i < boardSize; i++)
            {
                if (CellFree(i))
                {
                    availableMoves.Add(i);
                }
            }
            return availableMoves.OrderBy(x => rng.NextDouble()).ToArray();
        }
    }

    public int getTurn => currentTurn;

    public void PopOldest(bool nextTurn = false)
    {
        if (GameOver()) return;

        if (playHistory[currentTurn].TryDequeue(out int position))
        {
            board[currentTurn] &= ~(1 << position);
            onPop?.Invoke(position);
        }

        if (nextTurn) NextTurn();
    }
}