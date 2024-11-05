using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPack : MonoBehaviour, ICanCollect
{
    public int amount = 30;
    public AudioClip sound;

    public void Collect()
    {
        GunManager.Instance.AddBullet(amount);
        SoundManager.PlaySfx(sound);
        Destroy(gameObject);
    }
}
