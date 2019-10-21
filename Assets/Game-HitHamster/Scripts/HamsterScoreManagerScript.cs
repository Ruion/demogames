using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HamsterScoreManagerScript : MonoBehaviour {
	public TextMeshProUGUI scoreText;
	public int currentScore;

	void Awake () {
		currentScore = 0;
	}

	void Start(){
		UpdateScore();
	}
	
	public void AddScore(){
		currentScore += 1;
		string newScore = currentScore.ToString(); 
		scoreText.text = newScore;
		Debug.Log(newScore);
	}

	void UpdateScore(){
		Debug.Log("Update score");
		string newScore = currentScore.ToString(); 
		scoreText.text = newScore;
	}
}
