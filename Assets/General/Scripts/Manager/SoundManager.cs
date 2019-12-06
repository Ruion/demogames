using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource addScore;
    public AudioSource minusScore;

	public void AddScore(){ addScore.Play(); }
	public void MinusScore(){ minusScore.Play(); }
}
