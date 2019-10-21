using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterScript : MonoBehaviour {

	public enum Type{
		Hitable,Unhitable,Hostage
	}
	public Type hamsterType;
	public float maxAppealTime;
	private Animator animatorComponent;

	void Start(){
		StartCoroutine(RunAway(2f));
	}
	void OnMouseDown(){
        if (Input.GetMouseButtonDown(0)) {
			if(this.hamsterType == Type.Hitable){
				GameObject.Find("ScoreManager").GetComponent<HamsterScoreManagerScript>().AddScore();	
				StartCoroutine(DestroySelf(2f));
			}
            else if(this.hamsterType == Type.Unhitable){
				GameObject.Find("GameManager").GetComponent<HamsterGameManagerScript>().MinusTime(3);
				StartCoroutine(DestroySelf(2f));
			}
			else if(this.hamsterType == Type.Hostage){
				GameObject.Find("GameManager").GetComponent<HamsterGameManagerScript>().MinusTime(5);
				StartCoroutine(DestroySelf(2f));
			}
        }
	}

	IEnumerator DestroySelf(float animationTime){
		transform.Find("ExpAnimator").GetComponent<Animator>().Play("Explosion");
		yield return new WaitForSeconds(animationTime);
		GameObject.Find("HamsterManager").GetComponent<HamsterManagerScript>().RemovedInstance();
		Destroy(transform.gameObject);
	}

	IEnumerator RunAway(float animationTime){
		// TODO Show Animation
		yield return new WaitForSeconds(maxAppealTime);
		transform.Find("ExpAnimator").GetComponent<Animator>().Play("Explosion");
		yield return new WaitForSeconds(animationTime);
		GameObject.Find("HamsterManager").GetComponent<HamsterManagerScript>().RemovedInstance();
		Destroy(transform.gameObject);
	}
}
