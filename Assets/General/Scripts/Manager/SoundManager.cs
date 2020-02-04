using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource addScore;
    public AudioSource minusScore;

    public void AddScore() { addScore.PlayOneShot(addScore.clip); }
        
	public void MinusScore(){ minusScore.PlayOneShot(minusScore.clip); }
}
