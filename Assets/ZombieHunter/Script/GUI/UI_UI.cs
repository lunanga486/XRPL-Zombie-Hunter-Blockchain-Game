using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UI : MonoBehaviour
{
    [Header("GUN UI")]
    public Image gunIcon;
    public Text bulletLeft;
    [Space]
    public Text coinTxt;

    private void Update()
    {
        //healthSlider.value = Mathf.Lerp(healthSlider.value, healthValue, lerpSpeed * Time.deltaTime);
        
        coinTxt.text = GlobalValue.SavedCoins + "";
        bulletLeft.text = GameManager.Instance.Player.gunTypeID.bullet + "";
        gunIcon.sprite = GameManager.Instance.Player.gunTypeID.icon;
    }

    public void NextGun()
    {
        GunManager.Instance.NextGun();
    }
   
}
