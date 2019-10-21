using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(AudioSource))]
public class PrizeManagerScript : MonoBehaviour {

	[System.Serializable]
	public class Prize{
		public float point;
		public string prizeName;
	}
	[Header("Prize Setting")]
	[Tooltip("Declare all the available prize and relative criteria here")]
	public List<Prize> allPrize;
	[Tooltip("If true, prize is given if the player score is less than criteria score")]
	public bool lessthan = false;

	[Header("Result Panel Setting")]
	
	public AudioClip WinningSound;
	public AudioClip GameOverSound;
	public GameObject gameOverPanel;
	public Button btnBack;
	public Button btnClaim;
	[Header("Others")]
	public TextMeshProUGUI scoreText;

	private AudioSource audioSourceComponent;

	void Awake(){
		audioSourceComponent = GetComponent<AudioSource>();
	}

	public void GameOver(float totalPoint){
		bool havePrize = false;
		for(int i = 0;i < allPrize.Count; i++){
			if(lessthan){
				if(totalPoint <= allPrize[i].point){
					havePrize = true;
					PlayerPrefs.SetString("prizeName",allPrize[i].prizeName);
				}
			}
			else{
				if(totalPoint >= allPrize[i].point){
					havePrize = true;
					PlayerPrefs.SetString("prizeName",allPrize[i].prizeName);
				}
			}
		}

		// If prize to claim
		if(havePrize == true){
			btnClaim.interactable = true;
			audioSourceComponent.PlayOneShot(WinningSound);
		}
		else{
			
			audioSourceComponent.PlayOneShot(GameOverSound);
		}

		UpdateScore(totalPoint);
		gameOverPanel.SetActive(true);
	}

	void UpdateScore(float score){
		//scoreText.SetText("Your Score:" + "\n" + score.ToString());
	}
	
}
