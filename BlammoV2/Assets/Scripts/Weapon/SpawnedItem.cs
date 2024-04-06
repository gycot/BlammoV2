using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedItem : MonoBehaviour
{

    [System.NonSerialized]
    public Transform thisTransform;
    [System.NonSerialized]
    public SpawnedItem ParentItem;

    protected virtual void Awake()
    {
        thisTransform = transform;
    }
    protected virtual void Update()
    {

    }






}