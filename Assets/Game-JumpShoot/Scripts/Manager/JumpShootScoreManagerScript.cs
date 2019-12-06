using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpShootScoreManagerScript : GameSettingEntity {

	public Animator animator;
    public ScoreVisualizer scoreVisualizer;
	[ReadOnly] public float bestScore;

    public int JumpAddScore = 1;

    public string scoreName = "game_score";

    void Start () {

        scoreName = gameSettings.scoreName;
	}

	public void AddScore(){
        scoreVisualizer.UpdateText(JumpAddScore);

        animator.Play("1", 0, 0);
	}

}
