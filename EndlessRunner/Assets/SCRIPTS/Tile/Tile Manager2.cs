using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TileManager2 : MonoBehaviour
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
    public float fixedYPosition = 0f; // The constant Y position for the tiles
    private bool hasPlayerMoved = false; // Check if the player has moved along the Z axis

    public GameObject firstTilePrefab; // Reference to the specific tile prefab to spawn first
    private bool isFirstTileSpawned = false; // Flag to track if the first tile has been spawned after restart

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
    }


    void Start()
    {
        // Find the player object tagged as "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player transform not found!");
            return;
        }
        else
        {
            Debug.Log("Player transform found!");
        }

        // Start spawning tiles after initialization
        SpawnInitialTiles();
    }

    void Update()
    {
        // Check if the player has moved in the Z direction
        if (playerTransform != null && !hasPlayerMoved && playerTransform.position.z > 0f)
        {
            hasPlayerMoved = true;
            Debug.Log("Player has moved. Tiles will now spawn at a fixed Y position.");
        }

        // If the player has moved, spawn tiles at the constant Y position
        if (hasPlayerMoved)
        {
            if (playerTransform.position.z - tileGap > zSpawn - (tileAmount * tileLength))
            {
                // If the first tile has been spawned, spawn random tiles, else spawn the first specific tile
                if (!isFirstTileSpawned)
                {
                    SpawnRoadTile(firstTilePrefab); // Spawn the first specific tile
                    isFirstTileSpawned = true; // Set the flag to avoid spawning the first tile again
                }
                else
                {
                    SpawnRoadTile(Random.Range(0, tilePrefabs.Length)); // Spawn random tiles after the first
                }

                SpawnSideTile(currentCountry);
                Count++;
                DeleteTile();
            }
        }

        if (Count > countryAmount)
        {
            Count = 0;
            currentCountry++;
        }

        if (currentCountry >= countryTiles.Length)
            currentCountry = 0;
    }

    private void SpawnInitialTiles()
    {
        // Spawn the initial set of tiles
        for (int i = 0; i < tileAmount; i++)
        {
            if (i == 0)
                SpawnRoadTile(firstTilePrefab); // Spawn the specific tile first
            else
                SpawnRoadTile(Random.Range(0, tilePrefabs.Length)); // Spawn random tiles afterwards

            SpawnSideTile(0);
        }
    }

    public void ResetTiles()
    {
        // Reset the player moved trigger to false when restarting or resetting the game
        hasPlayerMoved = false;

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

        // Re-initialize tiles after resetting
        isFirstTileSpawned = false; // Reset the flag to ensure first tile is always the same after reset
        SpawnInitialTiles();
        currentCountry++;
    }

    // Method to spawn a specific tile
    public void SpawnRoadTile(GameObject tilePrefab)
    {
        // Spawn a specific road tile at a fixed Y position after the player has moved
        Vector3 spawnPosition = new Vector3(transform.position.x, fixedYPosition, zSpawn);
        GameObject tile = Instantiate(tilePrefab, spawnPosition, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    // Method to spawn a random tile
    public void SpawnRoadTile(int tileIndex)
    {
        // Spawn a random road tile at a fixed Y position after the player has moved
        Vector3 spawnPosition = new Vector3(transform.position.x, fixedYPosition, zSpawn);
        GameObject tile = Instantiate(tilePrefabs[tileIndex], spawnPosition, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    public void SpawnSideTile(int tileIndex)
    {
        // Spawn the side tile at a fixed Y position after the player has moved
        Vector3 spawnPosition = new Vector3(transform.position.x, fixedYPosition, zSpawnSide);
        GameObject sideTile = Instantiate(countryTiles[tileIndex], spawnPosition, transform.rotation);
        activeSideTiles.Add(sideTile);
        zSpawnSide += tileLength;
    }

    private void DeleteTile()
    {
        if (activeTiles.Count > 0)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }

        if (activeSideTiles.Count > 0)
        {
            Destroy(activeSideTiles[0]);
            activeSideTiles.RemoveAt(0);
        }
    }
}