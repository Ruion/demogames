using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpShootGameManagerScript : MonoBehaviour {

	public enum GameState{
		Menu,Playing,Shop
	};
	public static GameState gameState;
	public GameObject player;
	public GameObject groundHolder;
	public bool lost;
	private GameObject newGround;
	GroundScript newGroundScript;
	AudioManagerScript audioManager;
	AudioSource backgroundMusic;

    public GameManager GM;
    public JumpShootScoreManagerScript scoreM;
    public AudioManagerScript audioM;


    void Awake(){
		lost = false;
		Time.timeScale = 1f;
		gameState = GameState.Menu;
		audioManager = GameObject.Find("_AudioManager").GetComponent<AudioManagerScript>();
		backgroundMusic = GameObject.Find("_BackgroundMusic").GetComponent<AudioSource>();
	}

    /*
	void Update(){
		if (JumpShootGameManagerScript.gameState == GameState.Menu && Input.GetMouseButtonDown(0)){
			StartCoroutine(StartGame());
		}
	}
    */

    public void DoStartGame()
    {
        StartCoroutine(StartGame());
    }

	IEnumerator StartGame(){

        GM.StartGame();

		yield return new WaitForSeconds(0.2f);
		// player.GetComponent<Animator>().SetBool("PlayerActive",true);
		JumpShootGameManagerScript.gameState = GameState.Playing;

		yield break;
	}

    public void AddScore()
    {
        scoreM.AddScore();
        audioM.PlayCoinSound(); 

    }

	public void Dead(){
		StartCoroutine(DeadCoroutine());
	}

	public void GameOver(){

        GM.GameOver();
	}

	public void Revive(){
	//	Time.timeScale = 1f;
		newGround = groundHolder.transform.GetChild(0).gameObject;
		newGroundScript = newGround.GetComponent<GroundScript>();
		newGroundScript.velocity /= 2;
		newGroundScript.Stepped();

        player.transform.position = new Vector3(newGround.transform.position.x,newGround.transform.position.y+1,newGround.transform.position.z);
		player.GetComponent<JumpShootPlayerScript>().RevivePlayer();
	}

	IEnumerator DeadCoroutine(){
		audioManager.PlayDeadSound();
		backgroundMusic.Pause();
		//Time.timeScale = 0.1f;
		yield return new WaitForSecondsRealtime(0.5f);		
        GM.GameOver();
		lost = true;
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

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
