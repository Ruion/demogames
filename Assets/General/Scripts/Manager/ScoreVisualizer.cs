using UnityEngine;
using TMPro;

public class ScoreVisualizer : GameSettingEntity {

    public TextMeshProUGUI[] scoreTexts;
    private int score;
    [Header("Caution")]
    public bool clearScoreOnNewGame;

    public override void Awake()
    {
        base.Awake();
        score = 0;
    }

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

    public void VisualiseScore()
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = score.ToString();
        }
    }

    public void SaveScore()
    {
        LoadGameSettingFromMaster();

        string scoreName = gameSettings.scoreName;

        PlayerPrefs.SetString(scoreName, score.ToString());
    }

}
