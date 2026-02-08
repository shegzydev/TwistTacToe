using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelect : MonoBehaviour
{
    public enum ModeType
    {
        Classic, TickTacToe, Cursed, ThreeTacToe, FlipTacToe
    }

    [System.Serializable]
    public struct Mode
    {
        public ModeType type;
        public string description;
        public Sprite banner;

        public string name => type.ToString();
    }

    public List<Mode> modes = new()
    {
        new Mode
        {
            type = ModeType.Classic,
            description = "Play a classic game of TicTacToe"
        },
        new Mode
        {
            type = ModeType.TickTacToe,
            description = "Race against time every move, Your oldest move poofs after 3 seconds"
        },
        new Mode
        {
            type = ModeType.Cursed,
            description = "Watch your back, Anything can happen"
        },
        new Mode
        {
            type = ModeType.ThreeTacToe,
            description = "You can only have three symbols at once"
        }
    };
    int selectedMode = 0;

    [Header("UI")]
    [SerializeField] Image bannerImage;
    [SerializeField] TMP_Text nameText, descText;
    [SerializeField] Button nextButton, prevButton, playButton;

    void Start()
    {
        nextButton.onClick.AddListener(() =>
        {
            selectedMode++;
            if (selectedMode >= modes.Count) selectedMode = 0;
            SetMode();
        });

        prevButton.onClick.AddListener(() =>
        {
            selectedMode--;
            if (selectedMode < 0) selectedMode = modes.Count - 1;
            SetMode();
        });

        playButton.onClick.AddListener(() =>
        {
            LoadGameScene();
        });

        SetMode();
    }

    void Update()
    {

    }

    void SetMode()
    {
        var mode = modes[selectedMode];
        nameText.SetText(mode.name);
        descText.SetText(mode.description);
        bannerImage.sprite = mode.banner;

        Data.selectedMode = mode.type;
    }

    void LoadGameScene()
    {
        SceneManagement.Instance.Load("maingame");
    }
}
