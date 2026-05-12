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

    [Header("World Root")]
    [SerializeField] private Transform worldRoot;

    public float WorldSpeed { get; set; }

    private float elapsedTime;
    private float nextSpawnZ;
    private int sideIndex;
    private int sideCount;

    private List<GameObject> activeRoadTiles = new List<GameObject>();
    private List<GameObject> activeSideTiles = new List<GameObject>();

    private const int SIDE_CYCLE = 5;

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
        Debug.Log($"GM exists: {GameManager.Instance != null} | State: {GameManager.Instance?.CurrentState} | worldRoot: {worldRoot != null} | timeScale: {Time.timeScale}");

        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MoveWorld();
        TrySpawn();
        TryRecycle();
        UpdateSpeed();
    }

    private void InitialiseWorld()
    {
        elapsedTime = 0f;
        WorldSpeed = startSpeed;
        nextSpawnZ = 0f;
        sideIndex = 0;
        sideCount = 0;

        ClearAll();

        SpawnTile(firstTilePrefab ?? roadTilePrefabs[0], nextSpawnZ);
        SpawnSide(nextSpawnZ);
        nextSpawnZ += tileLength;

        while (activeRoadTiles.Count < tilesAhead)
            SpawnNext();
    }

    private void MoveWorld()
    {
        worldRoot.position -= new Vector3(0, 0, WorldSpeed * Time.deltaTime);
    }

    private void TrySpawn()
    {

        float frontEdge = nextSpawnZ + worldRoot.position.z;
        while (frontEdge < tilesAhead * tileLength)
        {
            SpawnNext();
            frontEdge = nextSpawnZ + worldRoot.position.z;
        }
    }

    private void TryRecycle()
    {
        if (activeRoadTiles.Count == 0) return;

        GameObject oldest = activeRoadTiles[0];
        float backEdge = oldest.transform.position.z;

        if (backEdge < -tileLength * 2f)
        {
            activeRoadTiles.RemoveAt(0);
            Destroy(oldest);

            if (activeSideTiles.Count > 0)
            {
                Destroy(activeSideTiles[0]);
                activeSideTiles.RemoveAt(0);
            }
        }
    }

    private void SpawnNext()
    {
        int index = Random.Range(0, roadTilePrefabs.Length);
        SpawnTile(roadTilePrefabs[index], nextSpawnZ);
        SpawnSide(nextSpawnZ);
        nextSpawnZ += tileLength;

        if (sideTilePrefabs.Length == 0) return;

        sideCount++;
        if (sideCount >= SIDE_CYCLE)
        {
            sideCount = 0;
            sideIndex = (sideIndex + 1) % sideTilePrefabs.Length;
        }
    }

    private void SpawnTile(GameObject prefab, float z)
    {
        Vector3 pos = new Vector3(0, 0, z);
        GameObject tile = Instantiate(prefab, pos, Quaternion.identity, worldRoot);
        activeRoadTiles.Add(tile);
    }

    private void SpawnSide(float z)
    {
        if (sideTilePrefabs.Length == 0) return;

        Vector3 pos = new Vector3(0, 0, z);
        GameObject side = Instantiate(sideTilePrefabs[sideIndex], pos, Quaternion.identity, worldRoot);
        activeSideTiles.Add(side);
    }

    private void UpdateSpeed()
    {
        elapsedTime += Time.deltaTime;
        WorldSpeed = Mathf.Lerp(startSpeed, maxSpeed, elapsedTime / timeToMaxSpeed);
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
        if (scene.name == "Main Game")
        {
            GameObject rootObj = GameObject.FindWithTag("WorldRoot");
            if (rootObj != null)
                worldRoot = rootObj.transform;
            else
                Debug.LogError("TileManager: WorldRoot not found in scene!");

            elapsedTime = 0f;
            InitialiseWorld();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}