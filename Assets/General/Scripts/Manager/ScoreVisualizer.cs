using UnityEngine;
using TMPro;

public class ScoreVisualizer : GameSettingEntity {

    public TextMeshProUGUI[] scoreTexts;
    private int score;
    public ScriptableScore scriptableScore;
    [Header("Caution")]
    public bool clearScoreOnNewGame;

    public override void Awake()
    {
        base.Awake();
        score = 0;
        if (clearScoreOnNewGame) { scriptableScore.score = 0; UpdateText(0); }
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
        float scoreF = score;

        scoreF = Mathf.Clamp(scoreF, scriptableScore.minimumScore, scriptableScore.maximumScore);

        score = (int)scoreF;
        VisualiseScore();

        scriptableScore.score = score;
    }

    public void VisualiseScore()
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = score.ToString();
        }
    }

    public void VisualiseScore(ScriptableScore scriptableScore)
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = scriptableScore.score.ToString();
        }
    }

    public void SaveScore()
    {
        LoadGameSettingFromMaster();

        string scoreName = gameSettings.scoreName;

        PlayerPrefs.SetString(scoreName, scriptableScore.score.ToString());
    }

}
