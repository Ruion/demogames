using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpShootScoreManagerScript : MonoBehaviour {
	public int currentScore = 0;
	public int totalScore = 0;
	public TextMeshPro totalScoreText;
	public TextMeshPro currentScoreText;
	public TextMeshPro bestScoreText;
	public TextMeshPro best;
	public Animator animator;
    public ScriptableScore scoreCard;
	[ReadOnly] public float bestScore;

    public int JumpAddScore = 10;
    public int CollectShoeAddScore = 15;

    public string scoreName = "score_jumpshoot";

    private void Awake()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();

        if (dm == null)
        {
            Debug.LogError("Game setting entity not found in scene");
        }
        else
        {
            dm.LoadSetting();
            scoreName = dm.gameSettings.scoreName;
        }
    }

    void Start () {

		currentScoreText.text = currentScore.ToString();
	}

	public void UpdateTotalScore(){
		// totalScoreText.text = totalScore.ToString() + " Gold Left";
	}

	public void SaveScore(){
       PlayerPrefs.SetString(scoreName, currentScore.ToString());
	}

	public void AddScore(){
        scoreCard.score += JumpAddScore;
        currentScore = scoreCard.score;
		currentScoreText.text = currentScore.ToString();

        animator.Play("1");
	}

    public void AddScoreShoe()
    {
        scoreCard.score += CollectShoeAddScore;
        currentScore = scoreCard.score;
        currentScoreText.text = currentScore.ToString();

        animator.Play("1");
    }

	public void ChangeColorToWhite(){
		bestScoreText.color = Color.white;
		currentScoreText.color = Color.white;
		best.color = Color.white;
	}

}
