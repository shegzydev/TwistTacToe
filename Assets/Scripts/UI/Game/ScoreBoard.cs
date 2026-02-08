using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TMP_Text[] scoreTexts;
    int[] scores = { 0, 0 };

    void Start()
    {
        ResetScores();
    }

    void Update()
    {

    }

    public void Increment(int winner)
    {
        scores[winner]++;
        scoreTexts[winner].SetText(scores[winner].ToString());
    }

    public void ResetScores()
    {
        scores[0] = 0;
        scores[1] = 0;

        scoreTexts[0].SetText(scores[0].ToString());
        scoreTexts[1].SetText(scores[1].ToString());
    }
}
