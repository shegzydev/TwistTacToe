using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [SerializeField] bool isBot;
    [SerializeField] ScoreBoard scoreBoard;
    [SerializeField] Button exitButton;

    private void Awake()
    {
        if(isBot == Data.isBotMode)
        {
            Instance = this;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            ScreenManager.ActivateScreen("confirmexit");
        });
        ResetUI();
    }

    void Update()
    {
        
    }

    public void HideExitButton()
    {
        exitButton.gameObject.SetActive(false);
    }

    public ScoreBoard GetScoreBoard => scoreBoard;


    public void ResetUI()
    {
        scoreBoard.ResetScores();
        exitButton.gameObject.SetActive(true);
    }
}
