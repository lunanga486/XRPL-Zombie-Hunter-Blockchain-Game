using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterUI : MonoBehaviour
{
    public GameObject helicopterLeft, helicopterRight;

    void Update()
    {
        if (HellicopterFinishPoint.Instance.isShowing && (Mathf.Abs( HellicopterFinishPoint.Instance.gameObject.transform.position.x - Camera.main.transform.position.x) > 5))
        {
            helicopterLeft.SetActive(HellicopterFinishPoint.Instance.gameObject.transform.position.x < GameManager.Instance.Player.transform.position.x);
            helicopterRight.SetActive(HellicopterFinishPoint.Instance.gameObject.transform.position.x > GameManager.Instance.Player.transform.position.x);
        }
        else
        {
            helicopterLeft.SetActive(false);
            helicopterRight.SetActive(false);
        }
    }
}
