using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyReviveManagerScript : MonoBehaviour {

	public GameObject gameOverPanel;
	public GameObject revivePage;
	public GameObject finishGamePage;
	public GameObject timeCounterObject;
	public TextMeshPro counterText;

	public int counter = 10;
	public Coroutine countTimerCoroutine;
	public void ReviveWithAds(){
		StopCoroutine(countTimerCoroutine);
		GameObject.Find("_AudioManager").gameObject.GetComponent<AudioManagerScript>().StopDeadSound();
		// GameObject.Find("UnityAds").gameObject.GetComponent<UnityAdsPlacementScript>().ShowAd();
	}
	public void ReviveWithGold(){
		StopCoroutine(countTimerCoroutine);
		Revive();
	}
	
	public void Revive(){
		gameOverPanel.SetActive(false);
		// Switch to finish game page, so that wont show again
		revivePage.SetActive(false);
		finishGamePage.SetActive(true);
		GameObject.Find("_AudioManager").gameObject.GetComponent<AudioManagerScript>().StopDeadSound();
		GameObject.Find("_GameManager").GetComponent<JumpShootGameManagerScript>().Revive();
	}
	public void StartCountingGameOver(){
		countTimerCoroutine = StartCoroutine(CountTimerCoroutine());
	}

	public void GameOverScreen(){
		revivePage.SetActive(false);
		finishGamePage.SetActive(true);
		GameObject.Find("_GameManager").GetComponent<JumpShootGameManagerScript>().GameOver();
	}

	IEnumerator CountTimerCoroutine(){
		timeCounterObject.GetComponent<Animator>().SetBool("TimerRunning",true);
		counterText.text = counter.ToString();
		// If dead before then ignore the 5 sec
		if(GameObject.Find("_GameManager").GetComponent<JumpShootGameManagerScript>().lost == true){
			GameOverScreen();
		}

		while(true){
			yield return new WaitForSecondsRealtime(1f);
			if(--counter < 0){
				GameOverScreen();
				timeCounterObject.GetComponent<Animator>().SetBool("TimerRunning",false);
				yield break;
			}
			counterText.text = counter.ToString();
		}
	}
}
