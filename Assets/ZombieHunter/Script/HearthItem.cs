using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthItem : MonoBehaviour, ICanCollect
{
    public int amount = 30;
    public AudioClip sound;

    public void Collect()
    {
        GameManager.Instance.Player.AddHearth(amount);
        SoundManager.PlaySfx(sound);
        Destroy(gameObject);
    }
}
