using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ADDP/Enemy AI/Smart Enemy Ground Control")]
public class SmartEnemyGrounded : Enemy, ICanTakeDamage {
    
	public bool isSocking{ get; set; }
	public bool isDead{ get; set; }

	float velocityXSmoothing = 0;
    
    [Header("New")]
	bool allowCheckAttack = true;

	EnemyMeleeAttack meleeAttack;
    SpawnItemHelper spawnItem;
    public bool isFacingRight { get { return transform.rotation.eulerAngles.y == 180; } }

    public override void Start ()
	{
		base.Start ();
		isPlaying = true;
		isSocking = false;
		meleeAttack = GetComponent<EnemyMeleeAttack> ();

		if (meleeAttack && meleeAttack.MeleeObj)
			meleeAttack.MeleeObj.SetActive (attackType == ATTACKTYPE.MELEE);

		spawnItem = GetComponent<SpawnItemHelper> ();
    }

	public override void Update ()
	{
		base.Update ();
		HandleAnimation ();

        if (enemyState != ENEMYSTATE.WALK || GameManager.Instance.State != GameManager.GameState.Playing)
        {
            return;
        }

        if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
            DetectPlayer(delayChasePlayerWhenDetect);
    }

	Vector2 finalSpeed;
    public virtual void LateUpdate(){
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        {
            return;
        }

        Vector2 _dir = GameManager.Instance.Player.transform.position - transform.position;
		var targetVelocity = _dir.normalized * moveSpeed ;
		finalSpeed.x = Mathf.SmoothDamp(finalSpeed.x, targetVelocity.x, ref velocityXSmoothing, 0.1f);
		finalSpeed.y = targetVelocity.y;
		if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING || enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
            finalSpeed = Vector2.zero;

        if (isStopping || isStunning)
            finalSpeed = Vector2.zero;

		//Debug.LogError(_dir.normalized);
		if ((finalSpeed.x > 0 && !isFacingRight) || (finalSpeed.x < 0 && isFacingRight))
            Flip();

        transform.Translate(finalSpeed * Time.deltaTime, Space.World);


        if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
        {
            CheckAttack();
        }
	}

	void Flip(){
		transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.x, isFacingRight () ? 180 : 0, transform.rotation.z));
	}
    
    public override void Stun(float time = 2)
    {
        base.Stun(time);
        StartCoroutine(StunCo(time));
    }

    IEnumerator StunCo(float time)
    {
        AnimSetTrigger("stun");
        isStunning = true;
        yield return new WaitForSeconds(time);
        isStunning = false;
    }

    public override void StunManuallyOn()
    {
        AnimSetTrigger("stun");
        isStunning = true;
    }

    public override void StunManuallyOff()
    {
        isStunning = false;
    }
    
    public override void DetectPlayer(float delayChase = 0)
    {
        base.DetectPlayer(delayChase);
    }
    
	public void CallMinion(){
		AnimSetTrigger ("callMinion");
		SetEnemyState (ENEMYSTATE.ATTACK);
		allowCheckAttack = false;
	}

	void CheckAttack(){
		//CHECK AND CALL MINION IF THIS ENEMY HAS SCRIPT CALLMINION
			switch (attackType) {
			
			case ATTACKTYPE.MELEE:
				if (meleeAttack.AllowAction ()) {
					if (meleeAttack.CheckPlayer (isFacingRight ())) {
						SetEnemyState (ENEMYSTATE.ATTACK);
						meleeAttack.Action ();
						AnimSetTrigger ("melee");
					} else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK) {
						SetEnemyState (ENEMYSTATE.WALK);
					}
				}
				break;

			
			default:
				break;
		}
	}

	void AllowCheckAttack(){
		allowCheckAttack = true;
	}

	void HandleAnimation(){
		AnimSetFloat("speed", Mathf.Abs(finalSpeed.x) > 0 ? 1 : 0);
		AnimSetBool ("isRunning", Mathf.Abs (finalSpeed.x) > walkSpeed);
        AnimSetBool("isStunning", isStunning);
    }

	public void SetForce(float x, float y){
        finalSpeed = new Vector3(x, y, 0);
    } 
    
	public void AnimMeleeAttackStart(){
		meleeAttack.Check4Hit ();
	}

	public void AnimMeleeAttackEnd(){
		meleeAttack.EndCheck4Hit ();
	}

	public override void Die ()
	{
		if (isDead)
			return;

		base.Die ();

		isDead = true;

		CancelInvoke ();

		var cols= GetComponents<BoxCollider2D>();
		foreach (var col in cols)
			col.enabled = false;

		if (spawnItem && spawnItem.spawnWhenDie)
			spawnItem.Spawn ();
        
        AnimSetBool("isDead", true);

        if (enemyEffect == ENEMYEFFECT.BURNING)
			return;

		if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY) {
			gameObject.SetActive (false);
			return;
		}
        
		StopAllCoroutines ();
            StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
    }

	public override void Hit (Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
	{
		if (!isPlaying || isStunning)
			return;

		base.Hit (force, pushBack, knockDownRagdoll, shock);
		if (isDead)
			return;

        AnimSetTrigger("hit");

        if (spawnItem && spawnItem.spawnWhenHit)
			spawnItem.Spawn ();

        if (knockDownRagdoll)
            ;
        else if (pushBack)
            StartCoroutine(PushBack(force));
        else if (shock)
            StartCoroutine(Shock());
        else
            ;
    }

	public override void KnockBack (Vector2 force, float stunningTime = 0)
	{
		base.KnockBack (force);

        SetForce(force.x, force.y);
    }

	public IEnumerator PushBack(Vector2 force){
		SetForce (force.x, force.y);

		if (isDead) {
			Die ();
			yield break;
		}
	}

    public IEnumerator Shock()
    {
        if (isDead)
        {
            Die();
            yield break;
        }
    }

    IEnumerator DisableEnemy(float delay){
		yield return new WaitForSeconds (delay);
        if (disableFX)
            SpawnSystemHelper.GetNextObject(disableFX, true).transform.position = transform.position + new Vector3(isFacingRight? -0.5f:0.5f,0.5f,0);
        gameObject.SetActive (false);
	}
}
