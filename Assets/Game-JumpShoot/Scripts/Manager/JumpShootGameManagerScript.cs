using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpShootGameManagerScript : MonoBehaviour {

	public enum GameState{
		Menu,Playing,Shop
	};
	public static GameState gameState;
	public GameObject menuPanel;
	public GameObject player;
	public GameObject groundHolder;
	public bool lost;
	private GameObject newGround;
	GroundScript newGroundScript;
	AudioManagerScript audioManager;
	AudioSource backgroundMusic;

    public GameManager GM;


	void Awake(){
		lost = false;
		Time.timeScale = 1f;
		gameState = GameState.Menu;
		audioManager = GameObject.Find("_AudioManager").GetComponent<AudioManagerScript>();
		backgroundMusic = GameObject.Find("_BackgroundMusic").GetComponent<AudioSource>();
	}

	void Update(){
		if (JumpShootGameManagerScript.gameState == GameState.Menu && Input.GetMouseButtonDown(0)){
			StartCoroutine(StartGame());
		}
	}

	IEnumerator StartGame(){

        GM.StartGame();

		yield return new WaitForSeconds(0.2f);
		// player.GetComponent<Animator>().SetBool("PlayerActive",true);
		JumpShootGameManagerScript.gameState = GameState.Playing;

		yield break;
	}

	public void Dead(){
		StartCoroutine(DeadCoroutine());
	}

	public void GameOver(){

        GM.GameOver();

        StartCoroutine(GameOverScreenCoroutine());

        
	}

	public void Revive(){
		audioManager.StopDeadSound();
		backgroundMusic.Play();
		Time.timeScale = 1f;
		newGround = groundHolder.transform.GetChild(0).gameObject;
		newGroundScript = newGround.GetComponent<GroundScript>();
		newGroundScript.velocity = 0;

		player.transform.position = new Vector3(newGround.transform.position.x,newGround.transform.position.y+1,newGround.transform.position.z);
		player.GetComponent<JumpShootPlayerScript>().RevivePlayer();
	}

	IEnumerator DeadCoroutine(){
		audioManager.PlayDeadSound();
		backgroundMusic.Pause();
		Time.timeScale = 0.1f;
		GameObject.Find("_ScoreManager").GetComponent<JumpShootScoreManagerScript>().UpdateTotalScore();
		yield return new WaitForSecondsRealtime(0.5f);		
		float score = GameObject.Find("_ScoreManager").GetComponent<JumpShootScoreManagerScript>().currentScore;
        GM.GameOver();
	//	GameObject.Find("_PrizeManager").GetComponent<PrizeManagerScript>().GameOver(score);
		lost = true;
	}

	IEnumerator GameOverScreenCoroutine(){
		if(lost == true){
			GameObject.Find("_ScoreManager").GetComponent<JumpShootScoreManagerScript>().SaveScore();
		}
		yield return new WaitForSecondsRealtime(0.5f);
		float score = GameObject.Find("_ScoreManager").GetComponent<JumpShootScoreManagerScript>().currentScore;
		GameObject.Find("_PrizeManager").GetComponent<PrizeManagerScript>().GameOver(score);
		GameObject.Find("_ScoreManager").GetComponent<JumpShootScoreManagerScript>().ChangeColorToWhite();
	}

	public void RestartGame(){
		Scene scene = SceneManager.GetActiveScene(); 
		SceneManager.LoadScene(scene.name);
	}

	public static bool isPlaying(){
		return JumpShootGameManagerScript.gameState == JumpShootGameManagerScript.GameState.Playing;
	}

	public static bool isMenu(){
		return JumpShootGameManagerScript.gameState == JumpShootGameManagerScript.GameState.Menu;
	}

	public static bool isShop(){
		return JumpShootGameManagerScript.gameState == JumpShootGameManagerScript.GameState.Shop;
	}
}
