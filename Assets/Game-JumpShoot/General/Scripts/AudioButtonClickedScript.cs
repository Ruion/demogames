using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class AudioButtonClickedScript : MonoBehaviour {

	public AudioClip clickedSound;
	private AudioSource audioSource;
	private Button button;

	void Awake(){
		button = GetComponent<Button> ();    
		audioSource = GetComponent<AudioSource>();
		if (button) {
            button.onClick.AddListener (PlayAudio);
        }
	}

	public void PlayAudio(){
		audioSource.PlayOneShot(clickedSound);
	}
}
