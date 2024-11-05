using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeWeapon : MonoBehaviour
{
    [Header("ANIMATION")]
    public AnimatorOverrideController animatorOverride;

    public float damage = 50;
    public float rate = 1f;
    public float radiusCheck = 1;
    public float delayToSync = 0.2f;
    public Transform checkPoint;
    public AudioClip soundSwap;
    public AudioClip soundAttack;
  [HideInInspector] public float lastAttackTime = -999;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkPoint.position, radiusCheck);
    }

}
