using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public bool followX = true;
    public bool followY = false;


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector2(followX ? Camera.main.transform.position.x : transform.position.x, followY ? Camera.main.transform.position.y : transform.position.y);
    }
}
