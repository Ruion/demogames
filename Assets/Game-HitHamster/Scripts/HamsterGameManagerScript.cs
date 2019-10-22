using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HamsterGameManagerScript : MonoBehaviour {

	[Header("Game Setting")]
	public float gameTime;

	
	[Header("Others")]
	public GameObject fadeText;
	public GameObject fadeTextHolder;
	public float currentTime;
	public TextMeshProUGUI currentTimeText;
	public bool lose;
	[ReadOnly] public int level;

    public GameManager GM;

	void Awake(){
		level = 0;
		lose = false;

        if(GM == null) GM = FindObjectOfType<GameManager>();
	}

	void Start(){
		
		Time.timeScale = 1.0F;
		Time.fixedDeltaTime = Time.timeScale * 0.02F;
		currentTime = gameTime;
		UpdateTimeBar();
        
	}

    public void StartGame()
    {
        StartCoroutine(EndGameCoroutine());
    }

	IEnumerator EndGameCoroutine(){
		while(true){
			yield return new WaitForSecondsRealtime(1f);
			currentTime -=1;
			level = (int)(getMaxLevel() - currentTime/10f) + 1;
			UpdateTimeBar();
			if(currentTime <= 0){
				currentTime = 0;
				UpdateTimeBar();
				EndGame();
				break;
			}
		}
	}

	public void AddDifficulty(){
		Time.timeScale += 0.005f;
	}

	public float getMaxLevel(){
		return gameTime/10f;
	}

	public int getCurrentLevel(){
		return level;
	}

	public void MinusTime(int time){
		currentTime -= time;
		GameObject newObject = Instantiate(fadeText,new Vector2(0,0),Quaternion.identity);
		newObject.transform.SetParent(fadeTextHolder.transform);
	}

	void UpdateTimeBar(){
		currentTimeText.text = currentTime.ToString();
	}

	void EndGame(){
		lose = true;
		int score = GameObject.Find("ScoreManager").GetComponent<HamsterScoreManagerScript>().currentScore;
	//	GameObject.Find("PrizeManager").GetComponent<PrizeManagerScript>().GameOver(score);
		Time.timeScale = 0f;
		Time.fixedDeltaTime = Time.timeScale * 0.02F;

        GM.GameOver();
	}

	public bool loseAlready(){
		return lose;
	}
}
