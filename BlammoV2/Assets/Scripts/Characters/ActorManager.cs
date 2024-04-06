using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public Transform Player;
    public Transform SpawnCenter;
    public Vector3 SpawnRange;

    public int NumGnomes = 10;
    public Actor GnomePrefab;

    Transform thisTransform;

    public void Awake()
    {
        thisTransform = transform;
    }

    public void Start()
    {
        for(int i=0; i<NumGnomes; i++)
        {
            Actor ac = GameObject.Instantiate<Actor>(GnomePrefab, thisTransform);
            ac.gameObject.SetActive(true);
        }

    }


    public Vector3 GetRespawnPosition()
    {
        Vector3 pos = SpawnCenter.position;
        pos.x += UnityEngine.Random.Range(-SpawnRange.x, SpawnRange.x);
        pos.y += UnityEngine.Random.Range(-SpawnRange.y, SpawnRange.y);
        pos.z += UnityEngine.Random.Range(-SpawnRange.z, SpawnRange.z);
        return pos;
    }
}