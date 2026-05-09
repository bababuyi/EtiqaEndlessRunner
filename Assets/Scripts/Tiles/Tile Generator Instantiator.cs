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
            Debug.Log("TileManager not found, instantiating...");
            Instantiate(tileManagerPrefab);
        }
        else
        {
            Debug.Log("TileManager already exists in the scene.");
        }
    }
}