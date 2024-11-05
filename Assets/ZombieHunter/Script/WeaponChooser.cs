using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChooser : MonoBehaviour
{
    public static WeaponChooser Instance;
    public Image gunTypeA, gunTypeB;
    public GunTypeID[] listGunA;
    public GunTypeID[] listGunB;

    private void Awake()
    {
        Instance = this;
    }

    //private void OnEnable()
    //{
    //    CheckGun();
    //}

    //private void Start()
    //{
    //    CheckGun();
    //}

    private void Update()
    {
        CheckGun();
    }

    bool hasGunA = false;
    bool hasGunB = false;
    void CheckGun()
    {
        if (!hasGunA)
        {
            foreach (var gunA in listGunA)
            {
                if (GlobalValue.isPicked(gunA))
                {
                    hasGunA = true;
                    gunTypeA.sprite = gunA.icon;
                }
            }

            if (!hasGunA)
            {
                gunTypeA.sprite = listGunA[0].icon;
                GlobalValue.pickGun(listGunA[0]);
            }
        }

        if (!hasGunB)
        {
            foreach (var gunB in listGunB)
            {
                if (GlobalValue.isPicked(gunB))
                {
                    hasGunB = true;
                    gunTypeB.sprite = gunB.icon;
                }
            }

            if (!hasGunB)
            {
                foreach (var gunB in listGunB)
                {
                    if (gunB.isUnlocked)
                    {
                        gunTypeB.sprite = gunB.icon;
                        GlobalValue.pickGun(gunB);

                        if (GunManager.Instance)
                            GunManager.Instance.ResetPlayerCarryGun();      //update the gun list if back to HomeScene from Playing scene
                    }
                }
            }
        }
    }

    public void SetGun(GunTypeID gunID)
    {
        if (gunID.gunType == GUNTYPE.typeA)
        {
            gunTypeA.sprite = gunID.icon;
        }
        else
        {
            gunTypeB.sprite = gunID.icon;
        }
    }

    public void PlayGame()
    {
        MainMenuHomeScene.Instance.LoadScene();
    }
}