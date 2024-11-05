using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance;
    public bool showTestOption = false;

    [Header("ITEM PRICE")]
    public int grenadePrice = 50;
    public int rocketPrice = 100;

    [Header("FPS DISPLAY")]
    public bool showInfor = true;
  [HideInInspector]  public Vector2 resolution = new Vector2(1280, 720);
    public int setFPS = 60;
    float deltaTime = 0.0f;

    public Purchaser purchase;

    public void BuyItem(int id)
    {
        switch (id)
        {
            case 1:
                purchase.BuyItem1();
                break;
            case 2:
                purchase.BuyItem2();
                break;
            case 3:
                purchase.BuyItem3();
                break;
            default:
                break;
        }
    }

    public void BuyRemoveAds()
    {
        purchase.BuyRemoveAds();
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = setFPS;

        if (ResourceBoost.Instance) {
            GlobalValue.SavedCoins += ResourceBoost.Instance.golds;
            ResourceBoost.Instance.ResetBoostValue();
        }     

    }

    #region FPS DISPLAY
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
        {
            GlobalValue.LevelPass = 999;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ResetDATA();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            GlobalValue.SavedCoins += 999999;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            GlobalValue.LevelPass = 999999;
        }
    }

    public void ResetDATA()
    {
        PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void OnGUI()
    {
        if (showInfor)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);


            GUI.Label(rect, text, style);

            Rect rect2 = new Rect(250, 0, w, h * 2 / 100);
            GUI.Label(rect2, Screen.currentResolution.width + "x" + Screen.currentResolution.height, style);
        }
    }
    #endregion
}
