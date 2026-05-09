using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TileManager : MonoBehaviour
{
    public GameObject firstTilePrefab; // The specific tile that always spawns first
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
    public float fixedYPosition = 0f;
    private bool hasPlayerMoved = false;

    private static TileManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (playerTransform == null)
            {
                Debug.LogError("Player transform not found!");
                return;
            }
        }

        if (tilePrefabs.Length == 0 || countryTiles.Length == 0)
        {
            Debug.LogError("Tile prefabs or country tiles not assigned!");
            return;
        }

        if (activeTiles.Count == 0)
        {
            SpawnInitialTiles();
        }
    }

    void Update()
    {
        if (playerTransform != null && !hasPlayerMoved && playerTransform.position.z > 0f)
        {
            hasPlayerMoved = true;
        }

        if (hasPlayerMoved)
        {
            if (playerTransform.position.z - tileGap > zSpawn - (tileAmount * tileLength))
            {
                SpawnRoadTile(Random.Range(0, tilePrefabs.Length));
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
        for (int i = 0; i < tileAmount; i++)
        {
            if (i == 0 && firstTilePrefab != null)
                SpawnSpecificTile(firstTilePrefab); // Spawn the specific first tile
            else
                SpawnRoadTile(Random.Range(0, tilePrefabs.Length));

            SpawnSideTile(0);
        }
    }

    public void ResetTiles()
    {
        hasPlayerMoved = false;

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

        zSpawn = 0;
        zSpawnSide = 0;

        SpawnInitialTiles();
        currentCountry++;
    }

    public void SpawnRoadTile(int tileIndex)
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, fixedYPosition, zSpawn);
        GameObject tile = Instantiate(tilePrefabs[tileIndex], spawnPosition, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    private void SpawnSpecificTile(GameObject tilePrefab)
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, fixedYPosition, zSpawn);
        GameObject tile = Instantiate(tilePrefab, spawnPosition, transform.rotation);
        activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    public void SpawnSideTile(int tileIndex)
    {
        if (tileIndex < 0 || tileIndex >= countryTiles.Length)
        {
            Debug.LogError($"SpawnSideTile: Invalid tileIndex {tileIndex}. Resetting to 0.");
            tileIndex = 0;
        }

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

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance != null && scene.name == "Main Game")
        {
            instance.ResetTiles();
        }
    }
}