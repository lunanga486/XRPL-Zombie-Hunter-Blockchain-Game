using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BARREL_TYPE { Rewarded, Explosion}
public class Barrel : MonoBehaviour,ICanTakeDamage
{
    public BARREL_TYPE barrelType; 
    protected HealthBarEnemyNew healthBar;
    public int health = 10;
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);
    int currentHealth;

    public GameObject blowFX;
    public Vector2 blowOffet = new Vector2(0, 0.5f);
    public AudioClip blowSound, hitSound;

    [Header("REWARDED")]
    public int amount = 2;
    public GameObject[] spawnItem;

    [Header("EXPLOSION")]
    public float radius = 3;
    public float damage = 20;
    public LayerMask targetLayer;

    [ReadOnly] public float activeDistance = 0;
    Collider2D coll2D;
    Animator anim;
    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        currentHealth -= (int)damage;

        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        if (currentHealth <= 0)
        {
            if (blowFX)
                Instantiate(blowFX, transform.position + (Vector3) blowOffet, blowFX.transform.rotation);
            SoundManager.PlaySfx(blowSound);

            if(barrelType == BARREL_TYPE.Rewarded && spawnItem.Length>0)
            {
                for(int i = 0; i < amount; i++)
                {
                    Instantiate(spawnItem[Random.Range(0, spawnItem.Length)], transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Quaternion.identity);
                }
            }
            else if (barrelType == BARREL_TYPE.Explosion)
            {
                var hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, 0, targetLayer);
                if (hits.Length > 0)
                {
                    foreach (var obj in hits)
                    {
                        obj.collider.gameObject.GetComponent<ICanTakeDamage>().TakeDamage(damage, Vector2.zero, obj.point, gameObject);
                    }
                }
            }
            gameObject.SetActive(false);
        }
        else
        {
            anim.SetTrigger("hit");
            SoundManager.PlaySfx(hitSound);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
        healthBar.Init(transform, (Vector3)healthBarOffset);
        currentHealth = health;
        activeDistance = 0.5f + Camera.main.aspect * Camera.main.orthographicSize;
        coll2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        coll2D.enabled = Mathf.Abs(transform.position.x - Camera.main.transform.position.x) < activeDistance;
        healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
    }
}
