using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    public List<string> helpTexts = new()
    {
        "Tap on an empty space to place your symbol",
        "Your goal is to get 3 in a row, horizontally, vertically, or diagonally, before your opponent",
        "If all spaces are filled without a winner, the game is a draw",
        "Whoever gets 3 ina row first, wins. Good luck!"
    };


    [SerializeField] TMP_Text tipText;
    [SerializeField] Button nextButton, prevButton;
    [SerializeField] Transform bannerHolder;
    int currentTip = 0;

    [SerializeField] Image[] scrollDots;
    [SerializeField] Color dotColorActive, dotColorInactive;

    void Start()
    {
        nextButton.onClick.AddListener(() =>
        {
            currentTip++;
            if (currentTip >= helpTexts.Count) currentTip = 0;
            DisplayTip();
        });

        prevButton.onClick.AddListener(() =>
        {
            currentTip--;
            if (currentTip < 0) currentTip = helpTexts.Count - 1;
            DisplayTip();
        });

        DisplayTip();
    }

    void Update()
    {
        
    }

    void DisplayTip()
    {
        foreach (Transform item in bannerHolder)
        {
            item.gameObject.SetActive(false);    
        }
        bannerHolder.GetChild(currentTip).gameObject.SetActive(true);

        for (int i = 0; i < scrollDots.Length; i++)
        {
            scrollDots[i].color = dotColorInactive;
        }
        scrollDots[currentTip].color = dotColorActive;

        tipText.SetText(helpTexts[currentTip]);
    }
}
