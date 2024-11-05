using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{
    public static RocketManager Instance;
    public RocketController rocket;
    public AudioClip sound;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        Instance = this;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void FireRocket()
    {
        SoundManager.PlaySfx(sound);
        var bounds = boxCollider.bounds;
        Instantiate(rocket.gameObject, new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y)), Quaternion.identity);
    }
}
