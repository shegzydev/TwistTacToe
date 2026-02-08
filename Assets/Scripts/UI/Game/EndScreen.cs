using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TMP_Text wonText;
    [SerializeField] Button home, replay;
    public event Action OnReplay = new Action(() => { });

    [SerializeField] GameObject starsHolder;

    void Start()
    {
        home.onClick.AddListener(() =>
        {
            SceneManagement.Instance.Load("menu");
        });

        replay.onClick.AddListener(() =>
        {
            OnReplay.Invoke();
            gameObject.SetActive(false);
        });
    }

    void Update()
    {

    }

    public void Show(bool won)
    {
        gameObject.SetActive(true);
        if (Data.isBotMode)
        {
            if (won) SoundManager.Instance.Play("win");
            wonText.SetText(won ? "YOU WON!" : "YOU LOST!");
            starsHolder.gameObject.SetActive(won);
        }
        else
        {
            wonText.SetText(won ? "RED\nWINS!!" : "BLUE\nWINS!");
        }
    }
}
