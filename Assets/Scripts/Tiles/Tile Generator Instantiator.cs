using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneratorInstantiator : MonoBehaviour
{
    public GameObject tileManagerPrefab; // Prefab for the TileManager
    private TileManager tileManager;

    void Start()
    {
        // Check if TileManager is already in the scene
        tileManager = FindObjectOfType<TileManager>();

        if (tileManager == null)
        {
            Debug.Log("TileManager not found, instantiating...");
            // Instantiate the TileManager if it doesn't exist
            Instantiate(tileManagerPrefab);
        }
        else
        {
            Debug.Log("TileManager already exists in the scene.");
        }
    }
}