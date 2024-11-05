using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour, ICanCollect
{
    [ReadOnly] public int rewarded = 5;
    public AudioClip sound;

    public void Init(int _rewarded)
    {
        rewarded = _rewarded;
    }

    public void Collect()
    {
        GlobalValue.SavedCoins += rewarded;
        SoundManager.PlaySfx(sound);
        FloatingTextManager.Instance.ShowText("+" + rewarded, transform.position, Vector2.zero, Color.yellow);
        gameObject.SetActive(false);
    }
}
