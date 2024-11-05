using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFortrest : MonoBehaviour, ICanTakeDamage
{
    public float maxHealth = 1000;
    public Sprite[] stateFortrestSprites;
    [ReadOnly] public int fortrestLevel = 1;
    public int[] enemyFortrestHealth;

    [ReadOnly] public float extraHealth = 0;
    [ReadOnly] public float currentHealth;

    
    public SpriteRenderer fortrestSprite;

    [Header("SHAKNG")]
    public float speed = 30f; //how fast it shakes
    public float amount = 0.5f; //how much it shakes
    public float shakeTime = 0.3f;
    public bool shakeX, shakeY;

    Vector2 startingPos;
    IEnumerator ShakeCoDo;

    void Awake()
    {
        startingPos = transform.position;
        fortrestSprite.sprite = stateFortrestSprites[0];
    }

    IEnumerator ShakeCo(float time)
    {
        float counter = 0;
        while (counter < time)
        {
            transform.position = startingPos + new Vector2(Mathf.Sin(Time.time * speed) * amount * (shakeX ? 1 : 0), Mathf.Sin(Time.time * speed) * amount * (shakeY ? 1 : 0));

            yield return null;
            counter += Time.deltaTime;
        }

        transform.position = startingPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    void UpdateHealth()
    {
        var healthValue = Mathf.Clamp01(currentHealth / maxHealth);
        FloatingTextManager.Instance.ShowText( (int)(healthValue * 100) + "%", transform.position, Vector2.up, Color.red);

    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        currentHealth -= damage;
        //FloatingTextManager.Instance.ShowText("" + (int)damage, transform.position, Vector2.up * 2, Color.yellow);
        UpdateHealth();
        if (currentHealth <= 0)
        {
            //if (healthCharacter == HEALTH_CHARACTER.PLAYER)
                GameManager.Instance.GameOver();
            //else
            //    GameManager.Instance.Victory();
        }
        else
        {
            if (ShakeCoDo != null)
                StopCoroutine(ShakeCoDo);

            ShakeCoDo = ShakeCo(shakeTime);
            StartCoroutine(ShakeCoDo);
        }

        //update fortrest state
        if (currentHealth > 0)
        {
            for (int i = (stateFortrestSprites.Length - 1); i > 0 ; i--)
            {
                if (currentHealth < ((maxHealth / (stateFortrestSprites.Length - 1)) * i))
                {
                    fortrestSprite.sprite = stateFortrestSprites[(stateFortrestSprites.Length - 1) - i];
                }
            }
        }
        else
            fortrestSprite.sprite = stateFortrestSprites[stateFortrestSprites.Length - 1];
    }
}
