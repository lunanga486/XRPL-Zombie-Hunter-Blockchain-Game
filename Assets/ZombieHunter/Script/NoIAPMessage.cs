using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoIAPMessage : MonoBehaviour
{
    public static NoIAPMessage Instance;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        panel.SetActive(false);
    }

  public void OpenPanel(bool open)
    {
        SoundManager.Click();
        panel.SetActive(open);
    }
}
