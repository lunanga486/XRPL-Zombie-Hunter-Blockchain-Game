using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public Text missionTxt;
    public Text missionInformation;
    public Text timerTxt;
    public GameObject TimerObj;
    Mission currentMission;
    void Start()
    {
        currentMission = LevelWave.Instance.CurrentMission();

        if(LevelWave.Instance.mission.showHelicopter == SHOW_HELICOPTER.Timer)
        {
            StartCoroutine(TimerCo(currentMission.timer));
        }else if (LevelWave.Instance.mission.showHelicopter == SHOW_HELICOPTER.OnStart)
        {
            TimerObj.SetActive(false);
        }

        if (currentMission.missionType == MISSION_TYPE.DefenseFence)
            missionTxt.text = "Defense the fence!";
        else if (currentMission.missionType == MISSION_TYPE.Survior)
            missionTxt.text = "Hold on and fight!";
        else if (currentMission.missionType == MISSION_TYPE.Normal)
            missionTxt.text = "Reach to the helicopter!";
        else if (currentMission.missionType == MISSION_TYPE.ProtectMan)
            missionTxt.text = "Rescue the babe";
        

        if (LevelWave.Instance.missionInformation == "")
        {
            if (currentMission.missionType == MISSION_TYPE.DefenseFence)
                missionInformation.text = "Defense the fence!";
            else if (currentMission.missionType == MISSION_TYPE.Survior)
                missionInformation.text = "Hold on and fight!";
            else if (currentMission.missionType == MISSION_TYPE.Normal)
                missionInformation.text = "Reach to the helicopter!";
            else if (currentMission.missionType == MISSION_TYPE.ProtectMan)
                missionInformation.text = "Rescue the babe";
        }
        else
            missionInformation.text = LevelWave.Instance.missionInformation;
    }

    IEnumerator TimerCo(float timer)
    {
        HellicopterFinishPoint.Instance.Hide();
        timerTxt.text = timer + "";

        while (timer > 0)
        {
            while (GameManager.Instance.State != GameManager.GameState.Playing)
                yield return null;

             yield return new WaitForSeconds(1);
            timer--;
            timerTxt.text = timer + "";
        }

        HellicopterFinishPoint.Instance.Show();
    }
}
