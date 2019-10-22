using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HamsterScoreManagerScript : MonoBehaviour {
	public TextMeshProUGUI scoreText;
	public int currentScore;
    ScoreVisualizer sv;
	void Awake () {
		currentScore = 0;
        sv = GetComponent<ScoreVisualizer>();
	}

	void Start(){
        //UpdateScore();
        sv.UpdateText(0);
    }
	
	public void AddScore(){
		currentScore += 1;
             sv.UpdateText(1);
	}

}
