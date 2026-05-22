using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneratorInstantiator : MonoBehaviour
{
    public GameObject tileManagerPrefab;
    private TileManager tileManager;

    void Start()
    {
        tileManager = FindFirstObjectByType<TileManager>();

        if (tileManager == null)
        {
            Instantiate(tileManagerPrefab);
        }
    }
}