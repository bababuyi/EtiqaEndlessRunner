using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject firstTilePrefab;
    [System.Serializable]
    public class TileEntry
    {
        public GameObject prefab;
        [Range(0, 3)] public int difficulty;
    }
    [SerializeField] private TileEntry[] roadTiles;

    [Header("Side Buildings")]
    [SerializeField] private GameObject[] sideTilePrefabs;
    [SerializeField] private float buildingSpacing = 8.26f;
    [SerializeField] private float buildingXOffset = 4.78f;
    [SerializeField] private float buildingYOffset = 4.06f;

    [Header("Streetlights")]
    [SerializeField] private GameObject[] streetlightPrefabs;
    [SerializeField] private float streetlightSpacing = 30f;
    [SerializeField] private float streetlightXOffset = 7.5f;
    [SerializeField] private float streetlightYOffset = 0f;
    [SerializeField] private Vector3 streetlightRotationLeft = new Vector3(0f, 90f, 0f);
    [SerializeField] private Vector3 streetlightRotationRight = new Vector3(0f, -90f, 0f);

    [Header("Sidewalk Props")]
    [SerializeField] private GameObject[] sidewalkPropPrefabs;
    [SerializeField] private float sidewalkPropSpacing = 15f;
    [SerializeField] private float sidewalkPropXOffset = 6.5f;
    [SerializeField] private float sidewalkPropYOffset = 0f;
    [SerializeField] private float sidewalkPropSpawnChance = 0.6f;

    private float nextBuildingZ = 0f;
    private float nextStreetlightZ = 0f;
    private float nextSidewalkPropZ = 0f;

    [Header("Settings")]
    [SerializeField] private int tilesAhead = 6;
    [SerializeField] private float tileLength = 45f;
    [SerializeField] private float startSpeed = 8f;
    [SerializeField] private float maxSpeed = 22f;
    [SerializeField] private float timeToMaxSpeed = 120f;
    [SerializeField][Range(0f, 1f)] private float transitionInfluence = 0.5f;

    [Header("Difficulty Progression")]
    [SerializeField] private int hardTilesBeforeBreather = 5;
    [SerializeField] private int breatherLength = 3;

    [Header("References")]
    [SerializeField] private Transform worldRoot;

    public float WorldSpeed { get; set; }

    private float nextSpawnZ;
    private float elapsedTime;
    private int consecutiveHardTiles = 0;
    private int breatherTilesLeft = 0;
    private int lastTileDifficulty = 0;

    private static readonly float[] EarlyWeights = { 0.70f, 0.25f, 0.05f, 0.00f };
    private static readonly float[] LateWeights = { 0.05f, 0.15f, 0.50f, 0.30f };

    private static readonly float[,] TransitionWeights =
    {
        { 0.50f, 0.35f, 0.15f, 0.00f },
        { 0.25f, 0.40f, 0.30f, 0.05f },
        { 0.10f, 0.50f, 0.30f, 0.10f },
        { 0.05f, 0.40f, 0.40f, 0.15f },
    };

    private readonly List<GameObject> activeRoadTiles = new();
    private readonly List<GameObject> activeSideTiles = new();

    private const float LEFT_LANE = -3f;
    private const float MIDDLE_LANE = 0f;
    private const float RIGHT_LANE = 3f;

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
        Debug.Log($"Tiles: {activeRoadTiles.Count}, nextSpawnZ: {nextSpawnZ}, worldZ: {worldRoot.position.z}");
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
        nextBuildingZ = 0f;

        ClearAll();

        GameObject firstTile = SpawnRoadTile(firstTilePrefab ?? roadTiles[0].prefab);
        SpawnSideBuildings();
        SpawnStreetlights();
        SpawnSidewalkProps();

        float measured = MeasureTileLength(firstTile);
        if (measured > 0.1f)
        {
            tileLength = measured;
            nextSpawnZ = tileLength;
        }

        while (activeRoadTiles.Count < tilesAhead)
            SpawnNextPair();
    }

    private void MoveWorld()
    {
        worldRoot.position -= new Vector3(0f, 0f, WorldSpeed * Time.deltaTime);
    }

    private void TrySpawn()
    {
        float cameraLocalZ = -worldRoot.position.z;

        while (nextSpawnZ < cameraLocalZ + (tilesAhead * tileLength))
        {
            SpawnNextPair();
        }
    }

    private void TryRecycle()
    {
        if (activeRoadTiles.Count == 0) return;

        float scrollOffset = -worldRoot.position.z;

        GameObject oldestTile = activeRoadTiles[0];
        if (oldestTile.transform.localPosition.z < scrollOffset - tileLength)
        {
            Destroy(oldestTile);
            activeRoadTiles.RemoveAt(0);
        }

        while (activeSideTiles.Count > 0)
        {
            GameObject oldest = activeSideTiles[0];
            if (oldest == null) { activeSideTiles.RemoveAt(0); continue; }

            if (oldest.transform.localPosition.z < scrollOffset - tileLength)
            {
                Destroy(oldest);
                activeSideTiles.RemoveAt(0);
            }
            else break;
        }
    }

    private void UpdateSpeed()
    {
        elapsedTime += Time.deltaTime;
        WorldSpeed = Mathf.Lerp(startSpeed, maxSpeed, elapsedTime / timeToMaxSpeed);
    }

    private GameObject PickRandomTile()
    {
        float t = Mathf.Clamp01(elapsedTime / timeToMaxSpeed);

        if (breatherTilesLeft > 0)
        {
            breatherTilesLeft--;
            if (breatherTilesLeft == 0) consecutiveHardTiles = 0;
            GameObject breatherTile = PickTileOfDifficulty(0, t);
            lastTileDifficulty = 0;
            return breatherTile;
        }

        GameObject picked = PickWeightedTile(t);

        int difficulty = GetDifficulty(picked);
        lastTileDifficulty = difficulty;

        if (difficulty >= 2)
        {
            consecutiveHardTiles++;
            if (consecutiveHardTiles >= hardTilesBeforeBreather)
                breatherTilesLeft = breatherLength;
        }
        else
        {
            consecutiveHardTiles = 0;
        }

        return picked;
    }

    private GameObject PickWeightedTile(float t)
    {
        var weightedPool = new List<(GameObject prefab, float weight)>();
        float totalWeight = 0f;

        foreach (TileEntry entry in roadTiles)
        {
            if (entry.prefab == null) continue;

            float timeWeight = Mathf.Lerp(EarlyWeights[entry.difficulty], LateWeights[entry.difficulty], t);
            float transitionWeight = TransitionWeights[lastTileDifficulty, entry.difficulty];
            float blended = Mathf.Lerp(timeWeight, transitionWeight, transitionInfluence);

            weightedPool.Add((entry.prefab, blended));
            totalWeight += blended;
        }

        if (weightedPool.Count == 0) return null;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var (prefab, weight) in weightedPool)
        {
            cumulative += weight;
            if (roll <= cumulative) return prefab;
        }

        return weightedPool[^1].prefab;
    }

    private GameObject PickTileOfDifficulty(int targetDifficulty, float t)
    {
        var matches = new List<GameObject>();

        foreach (TileEntry entry in roadTiles)
            if (entry.prefab != null && entry.difficulty == targetDifficulty)
                matches.Add(entry.prefab);

        if (matches.Count > 0)
            return matches[Random.Range(0, matches.Count)];

        return PickWeightedTile(t);
    }

    private int GetDifficulty(GameObject prefab)
    {
        foreach (TileEntry entry in roadTiles)
            if (entry.prefab == prefab) return entry.difficulty;
        return 0;
    }

    private void SpawnNextPair()
    {
        SpawnRoadTile(PickRandomTile());
        SpawnSideBuildings();
        SpawnStreetlights();
        SpawnSidewalkProps();
    }

    private GameObject SpawnRoadTile(GameObject prefab)
    {
        GameObject tile = Instantiate(prefab, worldRoot);
        tile.transform.localPosition = new Vector3(0f, 0f, nextSpawnZ);
        tile.transform.localRotation = Quaternion.identity;
        activeRoadTiles.Add(tile);
        nextSpawnZ += tileLength;
        return tile;
    }

    private float MeasureTileLength(GameObject tile)
    {
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Keeping current tileLength.");
            return tileLength;
        }

        Bounds combined = renderers[0].bounds;
        foreach (Renderer r in renderers)
            combined.Encapsulate(r.bounds);

        float measured = combined.size.z;
        Debug.Log($"TileManager: Auto-measured tile length = {measured:F2}");
        return measured;
    }

    [SerializeField] private float sideXOffset = 15f;

    private void SpawnSideBuildings()
    {
        if (sideTilePrefabs == null || sideTilePrefabs.Length == 0) return;

        while (nextBuildingZ < nextSpawnZ)
        {
            // Right side
            GameObject right = Instantiate(sideTilePrefabs[Random.Range(0, sideTilePrefabs.Length)], worldRoot);
            right.transform.localPosition = new Vector3(buildingXOffset, buildingYOffset, nextBuildingZ);
            right.transform.localRotation = Quaternion.Euler(-90f, 180f, 90f);
            activeSideTiles.Add(right);

            // Left side
            GameObject left = Instantiate(sideTilePrefabs[Random.Range(0, sideTilePrefabs.Length)], worldRoot);
            left.transform.localPosition = new Vector3(-buildingXOffset, buildingYOffset, nextBuildingZ);
            left.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f);
            activeSideTiles.Add(left);

            nextBuildingZ += buildingSpacing;
        }
    }

    private void SpawnStreetlights()
    {
        if (streetlightPrefabs == null || streetlightPrefabs.Length == 0) return;

        while (nextStreetlightZ < nextSpawnZ)
        {
            GameObject prefab = streetlightPrefabs[Random.Range(0, streetlightPrefabs.Length)];

            // Right side
            GameObject right = Instantiate(prefab, worldRoot);
            right.transform.localPosition = new Vector3(streetlightXOffset, streetlightYOffset, nextStreetlightZ);
            right.transform.localRotation = Quaternion.Euler(streetlightRotationRight);
            activeSideTiles.Add(right);

            // Left side
            GameObject left = Instantiate(prefab, worldRoot);
            left.transform.localPosition = new Vector3(-streetlightXOffset, streetlightYOffset, nextStreetlightZ);
            left.transform.localRotation = Quaternion.Euler(streetlightRotationLeft);
            activeSideTiles.Add(left);

            nextStreetlightZ += streetlightSpacing;
        }
    }

    private void SpawnSidewalkProps()
    {
        if (sidewalkPropPrefabs == null || sidewalkPropPrefabs.Length == 0) return;

        while (nextSidewalkPropZ < nextSpawnZ)
        {
            if (Random.value <= sidewalkPropSpawnChance)
            {
                bool spawnRight = Random.value > 0.5f;

                GameObject prefab = sidewalkPropPrefabs[Random.Range(0, sidewalkPropPrefabs.Length)];
                float side = spawnRight ? sidewalkPropXOffset : -sidewalkPropXOffset;
                float yRot = spawnRight ? Random.Range(-15f, 15f) : Random.Range(165f, 195f);

                GameObject prop = Instantiate(prefab, worldRoot);
                prop.transform.localPosition = new Vector3(side, sidewalkPropYOffset, nextSidewalkPropZ);
                prop.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
                activeSideTiles.Add(prop);
            }

            nextSidewalkPropZ += sidewalkPropSpacing;
        }
    }

    private void ClearAll()
    {
        foreach (var t in activeRoadTiles) if (t) Destroy(t);
        foreach (var t in activeSideTiles) if (t) Destroy(t);
        activeRoadTiles.Clear();
        activeSideTiles.Clear();

        nextSpawnZ = 0f;
        nextBuildingZ = 0f;
        nextStreetlightZ = 0f;
        nextSidewalkPropZ = 0f;
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