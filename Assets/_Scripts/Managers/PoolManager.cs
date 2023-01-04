using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolInfo> m_ListOfPool;
    
    
    #region Singleton

    private static PoolManager m_Instance;

    public static PoolManager Instance => m_Instance;


    private void Awake()
    {
        m_Instance = this;
    }

    #endregion
    private void Start()
    {
        foreach (var poolInfo in m_ListOfPool)
        {
            FillPool(poolInfo);
        }
    }

    private void FillPool(PoolInfo poolInfo)
    {
        for (int i = 0; i < poolInfo.Amount; i++)
        {
            var poolObj = Instantiate(poolInfo.Prefab, poolInfo.Container);
            poolObj.SetActive(false);
            poolInfo.Pool.Add(poolObj);
        }
    }

    public GameObject GetPoolObject(PoolObjectType poolObjectType)
    {
        var selectedPool = GetPoolByType(poolObjectType);
        var pool = selectedPool.Pool;

        GameObject instance;

        if (pool.Count > 0)
        {
            instance = pool[pool.Count - 1];
            pool.Remove(instance);
        }
        else
        {
            instance = Instantiate(selectedPool.Prefab, selectedPool.Container);
        }

        return instance;
    }

    public void CoolObject(GameObject obj, PoolObjectType type)
    {
        obj.SetActive(false);

        var selectedType = GetPoolByType(type);
        var selectedPool = selectedType.Pool;

        if (!selectedPool.Contains(obj))
        {
            selectedPool.Add(obj);
        }
    }

    private PoolInfo GetPoolByType(PoolObjectType poolObjectType)
    {
        foreach (var poolInfo in m_ListOfPool)
        {
            if (poolInfo.Type == poolObjectType) return poolInfo;
        }

        return null;
    }
}

public enum PoolObjectType
{
    Soldier1,
    Soldier2,
    Soldier3
}

[Serializable]
public class PoolInfo
{
    public PoolObjectType Type;
    public int Amount = 0;
    public GameObject Prefab;
    public Transform Container;
    
    [HideInInspector] public List<GameObject> Pool = new List<GameObject>();
}