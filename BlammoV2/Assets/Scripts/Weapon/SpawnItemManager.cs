using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItemManager : MonoBehaviour
{

    static public SpawnItemManager Instance;
    Dictionary<SpawnedItem, Stack<SpawnedItem>> Cache = new Dictionary<SpawnedItem, Stack<SpawnedItem>>();

    Transform thisTransform;

    private void Awake()
    {
        Instance = this;
        thisTransform = transform;
    }

    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
    }


    public SpawnedItem GetItem(SpawnedItem parentPrefab)
    {
        SpawnedItem curDart;
        if(Cache.ContainsKey(parentPrefab) && Cache[parentPrefab].Count>0)
        {
            curDart = Cache[parentPrefab].Pop();
        }
        else
        {
            curDart = GameObject.Instantiate<SpawnedItem>(parentPrefab, thisTransform);
            curDart.ParentItem = parentPrefab;
        }        
        curDart.gameObject.SetActive(true);
        return curDart;
    }

    public void CacheItem(SpawnedItem item)
    {
        if(!Cache.ContainsKey(item.ParentItem))
        {
            Cache.Add(item.ParentItem, new Stack<SpawnedItem>());
        }
        Cache[item.ParentItem].Push(item);
        item.gameObject.SetActive(false);
    }

}
