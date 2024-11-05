using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour, IListener
{
    public bool beginOnStart = false;
    public enum CALL_HELICOPTER { No, OnStart, OnEnd}
    public CALL_HELICOPTER callHelicopter;
    public EnemyWave enemyWave;
    [Header("Limit Camera Optional")]
    public bool useLimitOption = false;
    public float limitLeft = 8;
    public float limitRight = 4;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Instance.Player.gameObject)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        LevelEnemyManager.Instance.BeginWave(enemyWave);

        if (useLimitOption)
        {
            CameraFollow.Instance.TempLimitCamera(transform.position.x - limitLeft, transform.position.x + limitRight);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (useLimitOption)
        {
            Gizmos.DrawWireCube(new Vector2(((transform.position.x - limitLeft) + (transform.position.x + limitRight))*0.5f, transform.position.y), new Vector2(limitRight + limitLeft, 1));
        }
    }

    public void IPlay()
    {
        if (beginOnStart)
            SpawnEnemy();
    }

    public void ISuccess()
    {
    }

    public void IPause()
    {
    }

    public void IUnPause()
    {
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }
}
