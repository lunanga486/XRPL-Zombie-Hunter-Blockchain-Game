using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapControllerUI : MonoBehaviour {
	public RectTransform BlockLevel;
	public int howManyBlocks = 3;
	public float step = 720f;
	private float newPosX = 0;
	int currentPos = 0;
	public AudioClip music;

    public Button nextBtn, preBtn;

	void Start () {
        SetWorldNumber();
    }

    void SetWorldNumber()
    {
      
    }

    void Update()
    {
        nextBtn.interactable = (currentPos < howManyBlocks - 1);
        preBtn.interactable = (currentPos > 0);
    }

    void OnEnable(){
		SoundManager.PlayMusic (music);
		Debug.LogWarning ("ON ENALBE");

	}

	void OnDisable(){
		SoundManager.PlayMusic (SoundManager.Instance.musicsGame);
	}

    public void SetCurrentWorld(int world)
    {
        currentPos += (world - 1);

        newPosX -= step * (world - 1);
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

        SetMapPosition();

        SetWorldNumber();
    }

    public void SetMapPosition()
    {
        BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
    }

    bool allowPressButton = true;
    public void Next()
    {
        if (allowPressButton)
        {
            StartCoroutine(NextCo());
        }
    }

    IEnumerator NextCo()
    {
        allowPressButton = false;

        SoundManager.Click();

        if (newPosX != (-step * (howManyBlocks - 1)))
        {
            currentPos++;

            newPosX -= step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            
        }
        else
        {
            allowPressButton = true;
            yield break;

            //currentPos = 0;

            //newPosX = 0;
            //newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);


        }

        BlackScreenUI.instance.Show(0.15f);

        yield return new WaitForSeconds(0.15f);
        SetMapPosition();
        BlackScreenUI.instance.Hide(0.15f);

        SetWorldNumber();


        allowPressButton = true;

    }

    public void Pre()
    {
        if (allowPressButton)
        {
            StartCoroutine(PreCo());
        }
    }

    IEnumerator PreCo()
    {
        allowPressButton = false;
        SoundManager.Click();
        if (newPosX != 0)
        {
            currentPos--;

            newPosX += step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);


        }
        else
        {
            allowPressButton = true;
            yield break;
            //currentPos = howManyBlocks - 1;

            //newPosX = -999999;
            //newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

        }

        BlackScreenUI.instance.Show(0.15f);

        yield return new WaitForSeconds(0.15f);
        SetMapPosition();
        BlackScreenUI.instance.Hide(0.15f);

        SetWorldNumber();


        allowPressButton = true;

    }

	public void UnlockAllLevels(){
		GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);
		UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		SoundManager.Click ();
	}
}
