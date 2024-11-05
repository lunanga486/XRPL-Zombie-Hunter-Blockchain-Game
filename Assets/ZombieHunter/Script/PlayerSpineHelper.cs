using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpineHelper : MonoBehaviour
{
    [Header("Fire Point object")]
    public Transform firePointObj;

    private void Start()
    {

    }
    public Vector2 GetFireWorldPoint()
    {
        Vector3 _point;
            _point = firePointObj.position;

        return _point;
    }
}
