using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseItemUI : MonoBehaviour
{
    public Image icon;
    public GunTypeID gunID;
    public GameObject unlockBtn;
    public Image pick;
    
    Button ownerButton;

    private void Awake()
    {
        if (gunID != null)
        {
            icon.sprite = gunID.icon;
        }

        ownerButton = GetComponent<Button>();
    }

    void Update()
    {
        ownerButton.interactable = gunID.isUnlocked;
        unlockBtn.SetActive(!gunID.isUnlocked);
        pick.gameObject.SetActive(gunID.isUnlocked);
        pick.color = GlobalValue.isPicked(gunID) ? Color.white : Color.black;
    }

    public void OpenShop()
    {
        MainMenuHomeScene.Instance.OpenShop(true);
    }

    public void SetGun()
    {
        SoundManager.PlaySfx(SoundManager.Instance.chooseGun);
        GlobalValue.pickGun(gunID);

        if(GunManager.Instance)
            GunManager.Instance.ResetPlayerCarryGun();      //update the gun list if back to HomeScene from Playing scene

        WeaponChooser.Instance.SetGun(gunID);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        if (gunID != null)
        {
            icon.sprite = gunID.icon;
        }
    }
}
