using UnityEngine;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    [Tooltip("Litmited the camera moving within this box collider")]
    public bool followX = true;
    public bool followY = false;
    public float limitLeft = -5;
    public float limitRight = 50;
    public float limitDown = -3;
    public float limitUp = 5;
    public float smooth = 1;
    public Vector2 offset = Vector2.zero;
    [HideInInspector]
    public Vector2 _min, _max;

    [ReadOnly] public bool manualControl = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetDefaultLimit();
    }

    public void SetDefaultLimit()
    {
        _min = new Vector2(limitLeft, limitDown);
        _max = new Vector2(limitRight, limitUp);
    }

    void LateUpdate()
    {
        if (!manualControl)
            DoFollowPlayer();
    }

    public void DoFollowPlayer()
    {
        Vector2 focusPosition = (Vector2)GameManager.Instance.Player.transform.position + offset;

        focusPosition.x = Mathf.Clamp(focusPosition.x, _min.x + CameraHalfWidth, _max.x - CameraHalfWidth);
        focusPosition.y = Mathf.Clamp(focusPosition.y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

        if (!followX)
            focusPosition.x = transform.position.x;
        if (!followY)
            focusPosition.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, (Vector3)focusPosition + Vector3.forward * -10, smooth);
    }

    public float CameraHalfWidth
    {
        get { return (Camera.main.orthographicSize * ((float)Screen.width / Screen.height)); }
    }

    public void TempLimitCamera(float minX, float maxX)
    {
        _min.x = minX;
        _max.x = maxX;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 boxSize = new Vector2(limitRight - limitLeft, limitUp - limitDown);
        Vector2 center = (new Vector2(limitRight + limitLeft, limitUp + limitDown)) * 0.5f;
        Gizmos.DrawWireCube(center, boxSize);
    }
}
