using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabeFollower : MonoBehaviour, ICanTakeDamage
{
    [Header("SET UP")]
    public Animator anim;
    public float moveSpeed = 10;
    public float stopDistance = 2;
    public float limitAbovePos = -0.8f;
    public float limitBelowPos = -3.65f;
    public AudioClip soundHurt, soundDie;

    [Header("HEALTH")]
    [Range(0, 5000)]
    public int health = 100;
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);

    float currentHealth;
    protected HealthBarEnemyNew healthBar;
     public bool isFacingRight { get { return transform.rotation.eulerAngles.y == 0; } }

    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        currentHealth = health;
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

        healthBar.Init(transform, (Vector3)healthBarOffset);
    }
    Vector2 velocity;
    bool isMoving = false;
  public  bool moveToHelicopter = false;
   public Vector3 helicopterPos;

    void Update()
    {
        if (!moveToHelicopter && !isMoving && Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position) > stopDistance + 0.5f)
            isMoving = true;

        if (isMoving)
        {
            var dir = (moveToHelicopter? helicopterPos : GameManager.Instance.Player.transform.position) - transform.position;
            velocity = dir.normalized * moveSpeed;
            transform.Translate(velocity * Time.deltaTime, Space.World);

            if ((transform.position.x > GameManager.Instance.Player.transform.position.x && isFacingRight) || (transform.position.x < GameManager.Instance.Player.transform.position.x && !isFacingRight))
                Flip();

            if (moveToHelicopter)
            {
                if (Vector2.Distance(transform.position, helicopterPos) < 0.2f)
                    gameObject.SetActive(false);
            }
            else if (Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position) < stopDistance)
                isMoving = false;
        }
        anim.SetBool("isMoving", isMoving);
    }

    public void MoveToHelicopter(Vector2 pos)
    {
        isMoving = true;
        helicopterPos = pos;
        moveToHelicopter = true;
    }

    void Flip()
    {
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 180 : 0, 0);
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        currentHealth -= damage;
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
            anim.SetTrigger("dead");
            SoundManager.PlaySfx(soundDie);
        }
        else
        {
            anim.SetTrigger("hurt");
            SoundManager.PlaySfx(soundHurt);
        }
    }
}
