using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShowpoint : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        GameManager.Instance.Player.transform.position = transform.position;
    }
}
