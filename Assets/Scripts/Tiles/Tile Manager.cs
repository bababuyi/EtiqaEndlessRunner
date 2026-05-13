using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject firstTilePrefab;
    [SerializeField] private GameObject[] roadTilePrefabs;
    [SerializeField] private GameObject[] sideTilePrefabs;

    [Header("Settings")]
    [SerializeField] private int tilesAhead = 6;
    [SerializeField] private float tileLength = 30f;
    [SerializeField] private float startSpeed = 8f;
    [SerializeField] private float maxSpeed = 22f;
    [SerializeField] private float timeToMaxSpeed = 120f;

    [Header("References")]
    [SerializeField] private Transform worldRoot;

    public float WorldSpeed { get; set; }

    private float nextSpawnZ;
    private float elapsedTime;

    private readonly List<GameObject> activeRoadTiles = new();
    private readonly List<GameObject> activeSideTiles = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() => InitialiseWorld();

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MoveWorld();
        TrySpawn();
        TryRecycle();
        UpdateSpeed();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitialiseWorld()
    {
        elapsedTime = 0f;
        WorldSpeed = startSpeed;
        nextSpawnZ = 0f;

        ClearAll();

        SpawnRoadTile(firstTilePrefab ?? roadTilePrefabs[0]);
        SpawnSideTile();

        while (activeRoadTiles.Count < tilesAhead)
        {
            SpawnNextPair();
        }
    }
    
    private void MoveWorld()
    {
        worldRoot.position -= new Vector3(0f, 0f, WorldSpeed * Time.deltaTime);
    }

    private void TrySpawn()
    {
        while (nextSpawnZ + worldRoot.position.z < tilesAhead * tileLength)
        {
            SpawnNextPair();
        }
    }

    private void TryRecycle()
    {
        if (activeRoadTiles.Count == 0) return;

        if (activeRoadTiles[0].transform.position.z < -tileLength)
        {
            Destroy(activeRoadTiles[0]);
            activeRoadTiles.RemoveAt(0);

            if (activeSideTiles.Count > 0)
            {
                Destroy(activeSideTiles[0]);
                activeSideTiles.RemoveAt(0);
            }
        }
    }

    private void UpdateSpeed()
    {
        elapsedTime += Time.deltaTime;
        WorldSpeed = Mathf.Lerp(startSpeed, maxSpeed, elapsedTime / timeToMaxSpeed);
    }

    private void SpawnNextPair()
    {
        int index = Random.Range(0, roadTilePrefabs.Length);
        SpawnRoadTile(roadTilePrefabs[index]);
        SpawnSideTile();
    }

    private void SpawnRoadTile(GameObject prefab)
    {
        GameObject tile = Instantiate(prefab, worldRoot);

        tile.transform.localPosition = new Vector3(0f, 0f, nextSpawnZ);
        tile.transform.localRotation = Quaternion.identity;
        
        activeRoadTiles.Add(tile);
        nextSpawnZ += tileLength;
    }

    private void SpawnSideTile()
    {
        if (sideTilePrefabs == null || sideTilePrefabs.Length == 0) return;

        float sideZ = nextSpawnZ - tileLength;

        int index = Random.Range(0, sideTilePrefabs.Length);
        GameObject side = Instantiate(sideTilePrefabs[index], worldRoot);
        side.transform.localPosition = new Vector3(0f, 0f, sideZ);
        side.transform.localRotation = Quaternion.identity;

        activeSideTiles.Add(side);
    }

    private void ClearAll()
    {
        foreach (var t in activeRoadTiles) if (t) Destroy(t);
        foreach (var t in activeSideTiles) if (t) Destroy(t);
        activeRoadTiles.Clear();
        activeSideTiles.Clear();

        nextSpawnZ = 0f;
        if (worldRoot) worldRoot.position = Vector3.zero;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Game") return;

        GameObject root = GameObject.FindWithTag("WorldRoot");
        if (root != null)
            worldRoot = root.transform;
        else
            Debug.LogError("TileManager: no WorldRoot found in scene.");

        elapsedTime = 0f;
        InitialiseWorld();
    }
}