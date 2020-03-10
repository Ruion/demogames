using UnityEngine;
using TMPro;

/// <summary>
/// Visualize the score to TextMeshProUGUI text. Use this component
/// to add/minus score and display score to text
/// </summary>
public class ScoreVisualizer : MonoBehaviour
{
    public TextMeshProUGUI[] scoreTexts;
    private int score;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateText(2);
        }
    }

    public void UpdateText(int amount)
    {
        score += amount;

        VisualiseScore();
    }

    public void GetScoreFromPlayerPrefs()
    {
        score = System.Int32.Parse(PlayerPrefs.GetString("score"));
    }

    public void VisualiseScore()
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = score.ToString();
        }
    }

    public void SaveScore()
    {
        PlayerPrefs.SetString("score", score.ToString());
    }
}