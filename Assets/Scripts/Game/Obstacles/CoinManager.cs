﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using Random = UnityEngine.Random;


public class CoinManager : MonoBehaviour
{
    const float DISTANCE_BETWEEN_OBSTACLES = 8f;
    const float HEIGHT_RANDOM = 5f;
    const int MIN_COUNT = 3;
    public GameObject prefab;
    Vector3 pos = new Vector3(DISTANCE_BETWEEN_OBSTACLES, 0, 0);

    List<Coin> coins = new List<Coin>();

    private static CoinManager instance = null;

    public static CoinManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CoinManager>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Reset()
    {
        for (int i = 0; i < coins.Count; i++)
            Destroy(coins[i].gameObject);

        coins.Clear();

        pos.x = 0;

        InstantiateCoin();
    }

    public Coin GetNextCoin(Vector3 pos)
    {
        for (int i = 0; i < coins.Count; i++)
        {
            if (pos.x < coins[i].transform.position.x + 2f)
                return coins[i];
        }

        return null;
    }

    public bool IsColliding(Vector3 pos)
    {
        Collider2D collider = Physics2D.OverlapBox(pos, new Vector2(0.3f, 0.3f), 0);
        

        if (collider != null)
            return true;

        return false;
    }

    public void CheckAndInstatiate()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].CheckToDestroy();
        }

        while (coins.Count < MIN_COUNT)
            InstantiateCoin();
    }

    void InstantiateCoin()
    {
        pos.x += DISTANCE_BETWEEN_OBSTACLES;
        pos.y = Random.Range(-HEIGHT_RANDOM, HEIGHT_RANDOM);
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        go.transform.SetParent(transform, false);
        Coin coin = go.GetComponent<Coin>();
        coin.OnDestroy += OnObstacleDestroy;
        coins.Add(coin);
    }

    void OnObstacleDestroy(Coin coin)
    {
        coin.OnDestroy -= OnObstacleDestroy;
        coins.Remove(coin);
    }
}