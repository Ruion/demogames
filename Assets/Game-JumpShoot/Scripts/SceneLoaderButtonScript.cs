using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class SceneLoaderButtonScript : MonoBehaviour {

	[Header("Next Scene Setting")]
	[Tooltip("Next scene to visit after click")]
	public string sceneString;
	[Tooltip("If true, it will go to register form first before going to next scene")]
	public bool registerFirst = false;
	[Tooltip("Scene name of register form")]
	public string registerSceneString = "RegisterForm";
	private Button startButton;

	void Awake(){
		startButton = GetComponent<Button> ();    
		if (startButton) {
            startButton.onClick.AddListener (LoadScene);
        }
	}

	void LoadScene(){
		if(registerFirst == true){
			PlayerPrefs.SetString("nextScene",sceneString);
			SceneManager.LoadScene(registerSceneString);
		}else{
			SceneManager.LoadScene(sceneString);
		}
	}

}
