using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GUNTYPE { typeA, typeB}

public class GunTypeID : MonoBehaviour
{
    public bool unlockDefault = false;
    public GUNTYPE gunType;
    public string gunID = "gun ID";
    public Sprite icon;
    public int unlockPrice = 900;
    [Header("ANIMATION")]
    public AnimatorOverrideController animatorOverride;
    [Header("WEAPONS")]
    //public UpgradedCharacterParameter upgradedCharacterID;
    public int maxBullet = 99;
    public ShootingMethob shootingMethob;
    [Range(0, 100)]
    public int minPercentAffect = 90;
    public float rate = 0.2f;
    public float reloadTime = 2;
    [Range(0.5f, 1f)]
    public float accuracy = 0.9f;
    public GameObject shellFX;
    public Transform shellPoint;

    public AudioClip soundFire;
    [Range(0, 1)]
    public float soundFireVolume = 0.5f;
    public AudioClip shellSound;
    [Range(0, 1)]
    public float shellSoundVolume = 0.5f;
    public AudioClip reloadSound;
    [Range(0, 1)]
    public float reloadSoundVolume = 0.5f;
    public bool reloadPerShoot = false;
    public bool dualShot = false;
    public float fireSecondGunDelay = 0.1f;
    public bool isSpreadBullet = false;
    public int maxBulletPerShoot = 1;

    public GameObject muzzleTracerFX;
    public GameObject muzzleFX;

    public void ResetBullet()
    {
        bullet = maxBullet;
    }

    public int bullet
    {
        get { return PlayerPrefs.GetInt("gunID" + gunID, maxBullet); }
        set { PlayerPrefs.SetInt("gunID" + gunID, Mathf.Min(value, maxBullet)); }
    }

    public bool isUnlocked
    {
        get { return (PlayerPrefs.GetInt("isUnlocked" + gunID, 0) == 1) || unlockDefault; }
        set { PlayerPrefs.SetInt("isUnlocked" + gunID, value ? 1 : 0); }
    }

    [Header("UPGRADE")]
    [Space]
    public UpgradeStep[] UpgradeSteps;

    public int CurrentUpgrade
    {
        get
        {
            int current = PlayerPrefs.GetInt(gunID + "upgrade" + "Current", 0);
            if (current >= UpgradeSteps.Length)
                current = -1;   //-1 mean overload
            return current;
        }
        set
        {
            PlayerPrefs.SetInt(gunID + "upgrade" + "Current", value);
        }
    }

    public void UpgradeCharacter()
    {
        CurrentUpgrade++;
        UpgradeRangeDamage = UpgradeSteps[CurrentUpgrade].damage;
    }

    public int UpgradeRangeDamage
    {
        get { return PlayerPrefs.GetInt(gunID + "UpgradeRangeDamage", UpgradeSteps[0].damage); }
        set { PlayerPrefs.SetInt(gunID + "UpgradeRangeDamage", value); }
    }
}

[System.Serializable]
public class UpgradeStep
{
    public int price;
    public int damage;
}