using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagerBackup : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        // Generate initial tiles
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

    // Update is called once per frame
    void Update()
    {
        if (!isGameActive || playerTransform == null) return; // Stops spawning if game is not active

        // Spawn new tiles when needed
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

    // Call this function to stop spawning new tiles (e.g., during a game over)
    public void StopSpawning()
    {
        isGameActive = false;
    }

    // Call this function to reset all tiles and the tile manager
    public void ResetTiles()
    {
        Debug.Log("Resetting tiles...");

        // Stop tile spawning
        StopSpawning();

        // Destroy all active tiles
        foreach (GameObject tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();

        foreach (GameObject sideTile in activeSideTiles)
        {
            Destroy(sideTile);
        }
        activeSideTiles.Clear();

        // Reset spawning values
        zSpawn = 0;
        zSpawnSide = 0;

        // Restart the tile generation
        Start();
        isGameActive = true; // Allow new tiles to be spawned again
    }

    // Spawns a road tile at the specified index
    public void SpawnRoadTile(int tileIndex)
    {
        GameObject tile = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    // Spawns a side tile at the specified index
    public void SpawnSideTile(int tileIndex)
    {
        GameObject sideTile = Instantiate(countryTiles[tileIndex], transform.forward * zSpawnSide, transform.rotation);
        activeSideTiles.Add(sideTile);
        zSpawnSide += tileLength;
    }

    // Deletes the first tile from both the road and side tile lists
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
        Destroy(activeSideTiles[0]);
        activeSideTiles.RemoveAt(0);
    }
}