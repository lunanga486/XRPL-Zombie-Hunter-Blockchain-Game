using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWave : MonoBehaviour
{
    public static LevelWave Instance;
    [Header("------MISSION------")]
    public Mission mission;
    public string missionInformation = "";

    private void Awake()
    {
        Instance = this;
    }

    public Mission CurrentMission()
    {
        return mission;
    }
}

public enum MISSION_TYPE { Normal, Survior, DefenseFence, ProtectMan}
public enum SHOW_HELICOPTER { OnStart, Timer}
[System.Serializable]
public class Mission
{
    public MISSION_TYPE missionType;
    [Header("------SHOW HELICOPTER------")]
    public SHOW_HELICOPTER showHelicopter;
    public float timer = 180;
}