using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class JumpShootPlayerScript : MonoBehaviour {
	public enum PlayerState{
		Standing,Jumping,Falling
	}
	
	public GameObject playerParent;
	public GameObject shootEffectPrefab;
	public GameObject landingEffectPrefab;
	public ParticleSystem landingEffectPrefab2;
	public GameObject wallEffectPrefab;
	public float jumpSpeed = 10f;
	public float alwaysLeftSpeed = 3f;
	public float alwaysRightSpeed = 3f;
	public float throwSpeed = 1.5f;
	public float fallSpeed = 20f;
	public float stunTime = 1f;
	public float wallAdjustment;
	[ReadOnly] public bool isDead = false;
	[ReadOnly] public float previousPosXParent;
	[ReadOnly] public float ScreenRadiusInWorldX;
	[ReadOnly] public PlayerState currentPlayerState;
	[ReadOnly] public GroundScript.GroundType standingGroundType;
	[ReadOnly] public bool initialCollide;
	[ReadOnly] public bool allowSlowMotion;
	Rigidbody2D rigidBody2DComponent;
	BoxCollider2D boxCollider2D;
	Animator animatorComponent;

    public JumpShootGameManagerScript jsGM;
    public float repositionRatio = 2f;
    public CameraShake camShake;
    public float stepDistanceY;

    void Awake () {
		rigidBody2DComponent = GetComponent<Rigidbody2D>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		animatorComponent = GetComponent<Animator>();
		//currentPlayerState = PlayerState.Jumping;
		currentPlayerState = PlayerState.Standing;
	//	animatorComponent.SetBool("Jumping",true);
	}

	void Start(){
		initialCollide = true;
	}
	
	void Update () {
		if(JumpShootGameManagerScript.isPlaying()){
			GetInput();
			// SlowMotion();
			FallChecking();
			BounceAtWall();
			GetPreviousPositionOfParent();
			DeadCheck();
		}
	}

    
	void OnCollisionEnter2D(Collision2D target){

        GroundScript groundScriptComponent = target.gameObject.GetComponent<GroundScript>();
        if (groundScriptComponent == null) return;

        if (transform.position.y <= target.transform.position.y + stepDistanceY) return;

        SetPositionIntoParentBound(target.transform);
        // Get Standing Groud Type
        standingGroundType = groundScriptComponent.groundType;

		rigidBody2DComponent.velocity = Vector2.zero;
        rigidBody2DComponent.gravityScale = 0;
		currentPlayerState = PlayerState.Standing;
		animatorComponent.SetBool("Jumping",false);
		transform.SetParent(target.gameObject.transform);
        
        GetPreviousPositionOfParent();
		if(initialCollide == false){
			if(groundScriptComponent.GetStepped() == false){
                jsGM.AddScore();
                groundScriptComponent.Stepped();
                GameObject landingEffect = Instantiate(landingEffectPrefab, transform.position, Quaternion.identity);
                Destroy(landingEffect, 0.2f);

                // play circle burst
                landingEffectPrefab2.Play();
            }

            StartCoroutine(target.gameObject.GetComponent<GroundScript>().LandingEffect());
			
        }
		else
		{
			initialCollide = false;
		}
    }

    IEnumerator OnCollisionExit2D(Collision2D target){

        if (!gameObject.activeSelf) { StopAllCoroutines(); yield break; }

	    yield return new WaitForSeconds(0.1f);

        if (currentPlayerState == PlayerState.Jumping){
		    GameObject.Find("_GroundManager").GetComponent<GroundManagerScript>().GenerateGround();
           if( target.collider.GetComponent<GroundScript>()?.GetStepped() == true ) Destroy(target.gameObject, 0.05f) ;
	    }
	}

    void GetPreviousPositionOfParent(){
		previousPosXParent = transform.parent.transform.position.x;
	}

    void SetPositionIntoParentBound(Transform parent)
    {
        BoxCollider2D parentCollider = GetComponentInParent<BoxCollider2D>();

        if (transform.position.x > (parent.position.x + parentCollider.bounds.extents.x / repositionRatio) ) transform.position = new Vector2(transform.position.x - parentCollider.bounds.extents.x / repositionRatio, transform.position.y);
        else if (transform.position.x < (parent.position.x - parentCollider.bounds.extents.x / repositionRatio) ) transform.position = new Vector2(transform.position.x + parentCollider.bounds.extents.x / repositionRatio, transform.position.y);

    }

	float ParentVelocity(){
		return (transform.parent.transform.position.x - previousPosXParent) * throwSpeed / Time.deltaTime;
	}

	void BounceAtWall(){
		float screenWidth = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector2(Screen.width,0)).x - wallAdjustment;
		float transformX = rigidBody2DComponent.position.x;

		if(transformX < -screenWidth){
			rigidBody2DComponent.position = new Vector2(-screenWidth,rigidBody2DComponent.position.y);
			rigidBody2DComponent.velocity = new Vector2(-rigidBody2DComponent.velocity.x,rigidBody2DComponent.velocity.y);
			PlayWallBounceEffect();
		}

		if(transformX >= screenWidth){
			rigidBody2DComponent.position = new Vector2(screenWidth,rigidBody2DComponent.position.y);
			rigidBody2DComponent.velocity = new Vector2(-rigidBody2DComponent.velocity.x,rigidBody2DComponent.velocity.y);
			PlayWallBounceEffect();
		}
	}

	void GetInput(){
		if (Input.GetMouseButtonDown(0)){
			if(currentPlayerState == PlayerState.Jumping){
				// StartCoroutine(Fall());
			}
			else if(currentPlayerState == PlayerState.Standing){
                Jump();
			}
		}
	}

	void Jump(){

        transform.SetParent(playerParent.transform);

        GameObject.Find("_AudioManager").GetComponent<AudioManagerScript>().PlayJumpSound();
		boxCollider2D.enabled = false;
		currentPlayerState = PlayerState.Jumping;
		animatorComponent.SetBool("Jumping",true);
	
		if(standingGroundType == GroundScript.GroundType.JumpHigh){
			// rigidBody2DComponent.velocity = new Vector2(ParentVelocity(),jumpSpeed * 1.08f);
			rigidBody2DComponent.velocity = new Vector2(0,jumpSpeed * 1.08f);
		}else{
			// rigidBody2DComponent.velocity = new Vector2(ParentVelocity(),jumpSpeed);
			rigidBody2DComponent.velocity = new Vector2(0,jumpSpeed);
		}

        rigidBody2DComponent.gravityScale = 4;
        
	}

	void DeadCheck(){
		if(isDead == false && Camera.main.transform.position.y - transform.position.y > 8f){
            isDead = true;
            camShake.enabled = true;
            StopPlayer();
        	jsGM.Dead();
        //	jsGM.Revive();

        }
	}

	void StopPlayer(){
		rigidBody2DComponent.isKinematic = true;
	//	rigidBody2DComponent.velocity = new Vector2(0,0);
	}

	public void RevivePlayer(){
		ContinuePlayer();
		isDead = false;
	}

	void ContinuePlayer(){
		rigidBody2DComponent.isKinematic = false;
	}

	void PlayWallBounceEffect(){
		if(currentPlayerState == PlayerState.Jumping){
			GameObject wallEffect = Instantiate(wallEffectPrefab,transform.position, Quaternion.identity);
			Destroy(wallEffect,0.2f);
		}
	}

	void SlowMotion(){
		if (allowSlowMotion == true && Input.GetMouseButton(0)){
			// TODO, this will caused laggy
			Time.timeScale = 0.05f;
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}
		else{
			NormalMotion();
		}
	}

	void NormalMotion(){
		Time.timeScale = 1;
    	Time.fixedDeltaTime = 0.02F ;
	}

	void FallChecking(){
		if(rigidBody2DComponent.velocity.y <= 0){
			rigidBody2DComponent.isKinematic = false;
			boxCollider2D.enabled = true;
		}
	}

	IEnumerator Fall(){
		GameObject shootEffect = Instantiate(shootEffectPrefab,transform.position, Quaternion.identity);

		currentPlayerState = PlayerState.Falling;
		boxCollider2D.enabled = true;

		rigidBody2DComponent.isKinematic = true;
		rigidBody2DComponent.velocity = new Vector2(0,0);

		allowSlowMotion = true;

		yield return new WaitForSeconds(stunTime);

		// Revert back
		allowSlowMotion = false;
		NormalMotion();

		rigidBody2DComponent.isKinematic = false;
		rigidBody2DComponent.velocity = new Vector2(0,-fallSpeed);
		Destroy(shootEffect);
		yield break;
	}

}
