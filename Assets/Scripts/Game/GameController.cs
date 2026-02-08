using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class GameController : MonoBehaviour
{
    [Header("BackGround")]
    [SerializeField] Gradient turnGradient;
    float gradientTarget, gradientPin;
    [SerializeField] Image backGroundImage;

    [Header("UI")]
    [SerializeField] Sprite[] ox_Sprites = new Sprite[2];
    [SerializeField] Image[] slots = new Image[9];
    [SerializeField] Transform[] turnIndicators;
    [SerializeField] Transform linesHolder;

    [Header("TimerMode")]
    [SerializeField] Image timerImage;

    TicTacToe ticTacToe;
    AI bot;

    int playerTurn = 0;
    bool canPlay;

    [Header("SCREENS")]
    [SerializeField] EndScreen endScreen;

    void Start()
    {
        SoundManager.Instance.Stop("music");

        InitTicTacToe();

        InitBot();

        GameInput.OnInputDetected = (slot) =>
        {
            if (!canPlay) return;

            int turn = ticTacToe.getTurn;
            ticTacToe.MakeMove(slot.row, slot.column);

            //LerpTransformScale(turnIndicators[turn], 0, 0.15f);

            canPlay = false;
        };

        endScreen.OnReplay += () =>
        {
            ResetGame();
        };

        ResetGame();
    }

    private void Update()
    {
        gradientPin = Mathf.Lerp(gradientPin, gradientTarget, 5 * Time.deltaTime);
        backGroundImage.color = turnGradient.Evaluate(gradientPin);
    }

    private void InitBot()
    {
        if (!Data.isBotMode) return;

        bot = new AI(ticTacToe, 1);
        bot.OnComputeAI += (action) =>
        {
            StartCoroutine(action);
        };
    }

    private void InitTicTacToe()
    {
        ticTacToe = new();

        ticTacToe.onStartPopTimer += turn =>
        {
            if (turn != playerTurn && Data.isBotMode) return;

            StopPopTimer();
            popRoutine = StartCoroutine(WaitAndPopSlot());
        };

        ticTacToe.onPlayed += (board) =>
        {
            StopPopTimer();

            for (int i = 0; i < slots.Length; i++)
            {
                if ((board.o_board & (1 << i)) > 0)
                {
                    slots[i].sprite = ox_Sprites[0];
                    LerpTransformScale(slots[i].transform, 1, 0.25f);
                }
                else if ((board.x_board & (1 << i)) > 0)
                {
                    slots[i].sprite = ox_Sprites[1];
                    LerpTransformScale(slots[i].transform, 1, 0.25f);
                }
                else
                {
                    LerpTransformScale(slots[i].transform, 0, 0.1f);
                }
            }

            SoundManager.Instance.Play(ticTacToe.getTurn == 0 ? "x" : "o");
        };

        ticTacToe.onPop += pos =>
        {
            StartCoroutine(Pop());
            IEnumerator Pop()
            {
                yield return new WaitForSeconds(0.1f);
                SoundManager.Instance.Play("pop");
                LerpTransformScale(slots[pos].transform, 0, 0.1f);
            }
        };

        ticTacToe.onTurnChanged += turn =>
        {
            StartCoroutine(receiveTurnUpdate());
            gradientTarget = turn;

            IEnumerator receiveTurnUpdate()
            {
                LerpTransformScale(turnIndicators[1 - turn], 0, 0.05f);

                yield return new WaitForSeconds(0.3f);

                if (playerTurn == turn || !Data.isBotMode)
                {
                    canPlay = true;
                    LerpTransformScale(turnIndicators[turn], 1, 0.33f);
                }
            }
        };

        ticTacToe.onGameEnd += endData =>
        {
            if (endData.score != 0) SoundManager.Instance.Play("end");

            GameUI.Instance.GetScoreBoard.Increment(ticTacToe.getTurn);

            StartCoroutine(endGame());

            IEnumerator endGame()
            {
                if (endData.winOrientation >= 0)
                    LerpTransformScale(linesHolder.GetChild(endData.winOrientation), 1, 0.3f);

                yield return new WaitForSeconds(0.75f);

                if (endData.score == 0)
                {
                    ResetGame();
                }
                else
                {
                    GameUI.Instance.HideExitButton();
                    if (Data.selectedMode == ModeSelect.ModeType.FlipTacToe) endData.score *= -1;
                    endScreen.Show(endData.score > 0);
                }
            }
        };

        ticTacToe.Log += msg =>
        {
            Debug.Log(msg);
        };

        ticTacToe.onReverseBoard += action =>
        {
            StartCoroutine(reverse());
            IEnumerator reverse()
            {
                yield return new WaitForSeconds(0.3f);
                action();
            }
        };
    }

    void ResetGame()
    {
        ticTacToe.Reset();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = null;
            slots[i].transform.localScale = Vector3.zero;
        }

        turnIndicators[0].localScale = Vector3.zero;
        turnIndicators[1].localScale = Vector3.zero;

        if (playerTurn == ticTacToe.getTurn)
        {
            canPlay = true;
            LerpTransformScale(turnIndicators[playerTurn], 1, 0.33f);
        }
        else
        {
            if (!Data.isBotMode)
            {
                canPlay = true;
                LerpTransformScale(turnIndicators[1 - playerTurn], 1, 0.33f);
            }
        }

        bot?.Reset();

        gradientTarget = ticTacToe.getTurn;

        GameUI.Instance.ResetUI();

        foreach (Transform line in linesHolder)
        {
            LerpTransformScale(line, 0, 0.05f);
        }
    }

    void LerpTransformScale(Transform t, float scale, float time, Action OnDone = null)
    {
        StartCoroutine(SetScale(t, scale, time));

        float Overshoot(float t, float strength = 1.5f)
        {
            t -= 1f;
            return t * t * ((strength + 1f) * t + strength) + 1f;
        }

        IEnumerator SetScale(Transform t, float scale, float time)
        {
            var currentScale = t.localScale;
            var currentTime = 0f;

            while (currentTime < time)
            {
                float easedT = Mathf.Max(0, Overshoot(currentTime / time));
                t.localScale = Vector3.LerpUnclamped(currentScale, Vector3.one * scale, easedT);
                currentTime += Time.deltaTime;
                yield return null;
            }

            t.localScale = Vector3.one * scale;

            OnDone?.Invoke();
        }
    }

    Coroutine popRoutine;
    IEnumerator WaitAndPopSlot()
    {
        timerImage.gameObject.SetActive(true);
        float timer = 0f;
        float waitTime = 2f;

        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            timerImage.fillAmount = timer / waitTime;
            yield return null;
        }

        ticTacToe.PopOldest(true);
        timerImage.gameObject.SetActive(false);
    }

    void StopPopTimer()
    {
        if (popRoutine != null) StopCoroutine(popRoutine);
        timerImage.gameObject.SetActive(false);
    }
}