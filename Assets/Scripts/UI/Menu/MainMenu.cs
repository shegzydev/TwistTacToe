using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animator uiAnimator;
    [SerializeField] DifficultySlider difficultySlider;

    [SerializeField] Button playBot, playFriend;
    [SerializeField] Button helpButton;

    void Start()
    {
        SoundManager.Instance.Play("music");

        difficultySlider.value = PlayerPrefs.GetInt("difficulty", 1);

        playBot.onClick.AddListener(() =>
        {
            ScreenManager.ActivateScreen("modes");
            Data.difficultyLevel = Mathf.RoundToInt(difficultySlider.value);
            Data.isBotMode = true;
        });

        playFriend.onClick.AddListener(() =>
        {
            ScreenManager.ActivateScreen("modes");
            Data.difficultyLevel = Mathf.RoundToInt(difficultySlider.value);
            Data.isBotMode = false;
        });

        helpButton.onClick.AddListener(() =>
        {
            ScreenManager.ActivateScreen("help");
        });

        difficultySlider.onValueChanged.AddListener((val) =>
        {
            SetDifficultyAnimParameter(difficultySlider.value / 2);
        });

        SetDifficultyAnimParameter(difficultySlider.value / 2);
    }

    void SetDifficultyAnimParameter(float val)
    {
        uiAnimator.SetFloat("time", val);

        PlayerPrefs.SetInt("difficulty", Mathf.RoundToInt(difficultySlider.value));
        PlayerPrefs.Save();
    }

    void Update()
    {
        playBot.interactable = !difficultySlider.isLerping;
    }
}
