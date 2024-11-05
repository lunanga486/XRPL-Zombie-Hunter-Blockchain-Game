using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterUpgrade : MonoBehaviour
{
    public GunTypeID gunID;
    [Space]
    public Text
        currentRangeDamage, upgradeRangeDamageStep;

    public Text price;
    public GameObject lockedObj;
    public Text unlockPriceTxt;
    public GameObject dot;
    public GameObject dotHoder;
    List<Image> upgradeDots;

    public Sprite dotImageOn, dotImageOff;

    bool isMax = false;

    // Start is called before the first frame update
    void Start()
    {
        upgradeDots = new List<Image>();
        upgradeDots.Add(dot.GetComponent<Image>());
        for (int i = 1; i < gunID.UpgradeSteps.Length; i++)
        {
            upgradeDots.Add(Instantiate(dot, dotHoder.transform).GetComponent<Image>());
        }
       
        if (gunID.CurrentUpgrade + 1 >= gunID.UpgradeSteps.Length)
            isMax = true;

        UpdateParameter();
    }

    void UpdateParameter()
    {
        lockedObj.SetActive(!gunID.isUnlocked);
        unlockPriceTxt.text = "$" + gunID.unlockPrice;

        currentRangeDamage.text = gunID.UpgradeRangeDamage + "";
        if (isMax)
        {
            upgradeRangeDamageStep.enabled = false;
            price.text = "MAX";
        }

        else
        {
            price.text = gunID.UpgradeSteps[gunID.CurrentUpgrade + 1].price + "";
            upgradeRangeDamageStep.text = "-> " + gunID.UpgradeSteps[gunID.CurrentUpgrade + 1].damage;
        }
       
        SetDots(gunID.CurrentUpgrade + 1);
    }

    void SetDots(int number)
    {
        for (int i = 0; i < upgradeDots.Count; i++)
        {
            if (i < number)
                upgradeDots[i].sprite = dotImageOn;
            else
                upgradeDots[i].sprite = dotImageOff;
        }
    }

    public void Upgrade()
    {
        if (isMax)
            return;

        if (GlobalValue.SavedCoins >= gunID.UpgradeSteps[gunID.CurrentUpgrade + 1].price)
        {
            GlobalValue.SavedCoins -= gunID.UpgradeSteps[gunID.CurrentUpgrade + 1].price;
            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);

            gunID.UpgradeCharacter();


            if (gunID.CurrentUpgrade + 1 >= gunID.UpgradeSteps.Length)
                isMax = true;

            UpdateParameter();
        }
        else
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
    }

    public void UnlockPrice()
    {
        if(GlobalValue.SavedCoins >= gunID.unlockPrice)
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundUnlockGun);
            GlobalValue.SavedCoins -= gunID.unlockPrice;
            gunID.isUnlocked = true;
            UpdateParameter();
        }
    }
}
