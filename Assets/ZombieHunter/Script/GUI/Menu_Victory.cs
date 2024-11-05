using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;

/// <summary>
/// Handle Level Complete UI of Menu object
/// </summary>
public class Menu_Victory : MonoBehaviour {
	public GameObject Menu;
	public GameObject Restart;
	public GameObject Next;

	void Awake(){
		Menu.SetActive (false);
		Restart.SetActive (false);
		Next.SetActive (false);
    }

    IEnumerator Start()
    {
        SoundManager.PlaySfx(SoundManager.Instance.soundVictoryPanel);
        
        GlobalValue.LevelStar(GlobalValue.levelPlaying, GameManager.Instance.levelStarGot);
        yield return new WaitForSeconds(0.5f);

        Menu.SetActive(true);
        Restart.SetActive(true);
        
        Next.SetActive(LevelWave.Instance);
    }
}
