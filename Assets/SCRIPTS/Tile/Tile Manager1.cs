using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager1 : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject[] countryTiles;
    public float zSpawn = 0;
    public float zSpawnSide = 0;
    public float tileLength = 30;
    public int tileAmount = 5;
    public float tileGap = 35;
    public int countryAmount = 5;
    public int Count = 0;
    public int currentCountry = 0;

    private List<GameObject> activeTiles = new List<GameObject>();
    private List<GameObject> activeSideTiles = new List<GameObject>();

    public Transform playerTransform;
    bool isGameActive = true;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Check if playerTransform is assigned
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not found!");
        }

        // Ensure we only spawn the tiles once when the game starts
        if (playerTransform != null)
        {
            for (int i = 0; i < tileAmount; i++)
            {
                if (i == 0)
                    SpawnRoadTile(0);
                else
                    SpawnRoadTile(Random.Range(0, tilePrefabs.Length));

                SpawnSideTile(0);
            }
            currentCountry++;
        }

    }

    void Update()
    {
        if (!isGameActive || playerTransform == null) return;

        if ((playerTransform.position.z - tileGap > zSpawn - (tileAmount * tileLength)) &&
            (playerTransform.position.z - tileGap > zSpawnSide - (tileAmount * tileLength)))
        {
            SpawnRoadTile(Random.Range(0, tilePrefabs.Length));
            SpawnSideTile(currentCountry);
            Count++;
            DeleteTile();
        }

        if (Count > countryAmount)
        {
            Count = 0;
            currentCountry++;
        }

        if (currentCountry >= countryTiles.Length)
            currentCountry = 0;
    }

    public void StopSpawning()
    {
        isGameActive = false;
    }

    public void ResetTiles()
    {
        // Destroy existing tiles when restarting or resetting the scene
        foreach (var tile in activeTiles)
        {
            Destroy(tile);
        }

        foreach (var sideTile in activeSideTiles)
        {
            Destroy(sideTile);
        }

        activeTiles.Clear();
        activeSideTiles.Clear();

        // Reset spawn positions
        zSpawn = 0;
        zSpawnSide = 0;

        // Optionally, you can re-initialize tiles after resetting
        for (int i = 0; i < tileAmount; i++)
        {
            if (i == 0)
                SpawnRoadTile(0);
            else
                SpawnRoadTile(Random.Range(0, tilePrefabs.Length));

            SpawnSideTile(0);
        }

        currentCountry++;
    }

    public void SpawnRoadTile(int tileIndex)
    {
        GameObject tile = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    public void SpawnSideTile(int tileIndex)
    {
        GameObject sideTile = Instantiate(countryTiles[tileIndex], transform.forward * zSpawnSide, transform.rotation);
        activeSideTiles.Add(sideTile);
        zSpawnSide += tileLength;
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
        Destroy(activeSideTiles[0]);
        activeSideTiles.RemoveAt(0);
    }
}