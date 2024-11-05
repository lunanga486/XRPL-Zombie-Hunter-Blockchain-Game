using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderLayer : MonoBehaviour
{
    public enum UPDATE_TYPE { None, Update, OnEnable }
    public UPDATE_TYPE updateType;
    [Tooltip("+ order layer this value")]
    public int offsetOrder = 0;
    float step = 0.05f;
    public SpriteRenderer spriteRenderer;
    public MeshRenderer skeleton;

    private void OnEnable()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;

        if (skeleton != null)
        {
            skeleton.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;
        }

        switch (updateType)
        {
            case UPDATE_TYPE.None:
                Destroy(this);
                break;
            case UPDATE_TYPE.Update:
                InvokeRepeating("UpdateCo", 0, 0.2f);
                break;
            case UPDATE_TYPE.OnEnable:
                break;
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void UpdateCo()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;

        if (skeleton != null)
        {
            skeleton.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;

        if (skeleton != null)
        {
            skeleton.sortingOrder = Mathf.RoundToInt((transform.position.y / step) * -1) + offsetOrder;
        }
    }
}
