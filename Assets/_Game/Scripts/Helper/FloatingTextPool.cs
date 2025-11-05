using System.Collections.Generic;
using Data;
using UnityEngine;

public class FloatingTextPool : Singleton<FloatingTextPool>
{
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private int poolSize = 20;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(floatingTextPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        else
            return Instantiate(floatingTextPrefab, transform);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}