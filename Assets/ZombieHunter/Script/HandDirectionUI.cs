using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDirectionUI : MonoBehaviour
{
    public GameObject leftHand, rightHand;

    void OnEnable()
    {
        if (GameManager.Instance && HellicopterFinishPoint.Instance)
        {
            leftHand.SetActive(HellicopterFinishPoint.Instance.gameObject.transform.position.x < GameManager.Instance.Player.transform.position.x);
            rightHand.SetActive(HellicopterFinishPoint.Instance.gameObject.transform.position.x > GameManager.Instance.Player.transform.position.x);
        }
    }
}
