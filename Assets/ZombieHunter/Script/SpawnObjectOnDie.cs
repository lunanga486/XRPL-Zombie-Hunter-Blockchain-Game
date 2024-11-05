using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnDie : MonoBehaviour
{
    [Header("COIN")]
    [Range(0f, 1f)]
    public float chanceSpawnMoney = 0.2f;
    public int coinGiveMin = 5;
    public int coinGiveMax = 10;

    public GameObject coinObj;

    [Header("FORCE SPAWN ITEM")]
    public GameObject forceItem;

    public void Spawn()
    {
        if (Random.Range(0f, 1f) <= chanceSpawnMoney)
        {
            int random = Random.Range(coinGiveMin, coinGiveMax);
            var coin = SpawnSystemHelper.GetNextObject(coinObj, true);
            coin.GetComponent<CoinItem>().Init(random);
            coin.transform.position = transform.position;
        }

        if (forceItem)
            Instantiate(forceItem, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.1f, 0.1f), 0), Quaternion.identity);
    }
}
