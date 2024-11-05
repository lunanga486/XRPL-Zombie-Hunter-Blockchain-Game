using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnemyManager : MonoBehaviour, IListener
{
    public static LevelEnemyManager Instance;
    public Transform spawnPositionA, spawnPositionB;
    public GameObject dunGround;
    EnemyWave enemyWave;
    int currentWave = 0;

    List<GameObject> listEnemySpawned = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void BeginWave(EnemyWave wave)
    {
        enemyWave = wave;

        StartCoroutine(SpawnEnemyCo());
    }

    IEnumerator SpawnEnemyCo()
    {
        yield return new WaitForSeconds(enemyWave.wait);

        while (GameManager.Instance.State != GameManager.GameState.Playing)
            yield return null;

        for (int j = 0; j < enemyWave.enemySpawns.Length; j++)
        {
            var enemySpawn = enemyWave.enemySpawns[j];
            yield return new WaitForSeconds(enemySpawn.wait);
            for (int k = 0; k < enemySpawn.numberEnemy; k++)
            {
                var spawnPos = GetSpawnPosition(enemySpawn.spawnPos);
                GameObject _temp = Instantiate(enemySpawn.enemy[Random.Range(0, enemySpawn.enemy.Length)], spawnPos, Quaternion.identity) as GameObject;
                var isEnemy = (Enemy)_temp.GetComponent(typeof(Enemy));
                if (isEnemy != null)
                {
                    if (enemySpawn.customHealth > 0)
                        isEnemy.health += (int)(isEnemy.health * enemySpawn.customHealth);
                    if (enemySpawn.customSpeed > 0)
                    {
                        float extraSpeed = (isEnemy.walkSpeed * enemySpawn.customSpeed);

                        isEnemy.walkSpeed += enemySpawn.randomSpeed ? Random.Range(0f, extraSpeed) : extraSpeed;
                    }
                    if (enemySpawn.customAttackDmg > 0)
                    {
                        var rangeAttack = _temp.GetComponent<EnemyRangeAttack>();
                        if (rangeAttack)
                            rangeAttack.damage += (rangeAttack.damage * enemySpawn.customAttackDmg);
                        var meleeAttack = _temp.GetComponent<EnemyMeleeAttack>();
                        if (meleeAttack)
                            meleeAttack.dealDamage += (meleeAttack.dealDamage * enemySpawn.customAttackDmg);
                        var throwAttack = _temp.GetComponent<EnemyThrowAttack>();
                        if (throwAttack)
                            throwAttack.damage += (throwAttack.damage * enemySpawn.customAttackDmg);
                    }

                }

                _temp.SetActive(false);

                if (enemySpawn.spawnPos == SpawnPos.Middle)
                {
                    SpawnSystemHelper.GetNextObject(dunGround, spawnPos, true);
                    yield return new WaitForSeconds(1);
                }
                else
                    yield return new WaitForSeconds(0.1f);

                _temp.SetActive(true);

                if (enemySpawn.spawnPos == SpawnPos.Middle)
                    isEnemy.SpawnUp();

                listEnemySpawned.Add(_temp);

                yield return new WaitForSeconds(Random.Range( enemySpawn.rateMin, enemySpawn.rateMax));
               
            }
        }

        //check all enemy killed
        while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }

        yield return new WaitForSeconds(0.5f);

        CameraFollow.Instance.SetDefaultLimit();
        MenuManager.Instance.ShowHandDirection();
    }

    bool isEnemyAlive()
    {
        for(int i = 0; i< listEnemySpawned.Count;i++)
        {
            if (listEnemySpawned[i].gameObject != null && listEnemySpawned[i].activeInHierarchy)
                return true;
        }

        return false;
    }

    public Vector2 GetSpawnPosition(SpawnPos spawnPos)
    {
        Vector2 _pos = Vector2.zero;
        switch (spawnPos)
        {
            case SpawnPos.Left:
                _pos = new Vector2(Camera.main.ViewportToWorldPoint(Vector3.zero).x, Random.Range(spawnPositionA.position.y, spawnPositionB.position.y));
                break;
            case SpawnPos.Right:
                _pos = new Vector2(Camera.main.ViewportToWorldPoint(Vector3.one).x, Random.Range(spawnPositionA.position.y, spawnPositionB.position.y));
                break;
            case SpawnPos.LeftAndRight:
                if(Random.Range(0,2) == 0)
                _pos = new Vector2(Camera.main.ViewportToWorldPoint(Vector3.zero).x, Random.Range(spawnPositionA.position.y, spawnPositionB.position.y));
                else
                    _pos = new Vector2(Camera.main.ViewportToWorldPoint(Vector3.one).x, Random.Range(spawnPositionA.position.y, spawnPositionB.position.y));
                break;
            case SpawnPos.Middle:
                _pos = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.1f,0.9f),0,0)).x, Random.Range(spawnPositionA.position.y, spawnPositionB.position.y));
                break;
        }

        return _pos;
    }

    public void IGameOver()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IPlay()
    {
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }
}

public enum SpawnPos { LeftAndRight, Right, Left , Middle }
[System.Serializable]
public class EnemyWave
{
    public float wait = 3;
    
    public EnemySpawn[] enemySpawns;
}


