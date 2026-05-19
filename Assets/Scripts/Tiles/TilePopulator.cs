using UnityEngine;

public class TilePopulator : MonoBehaviour
{
    [Header("Vehicle Prefabs")]
    [SerializeField] private GameObject[] vehiclePrefabs;

    [Header("Obstacle Type Prefabs")]
    [SerializeField] private GameObject[] jumpObstaclePrefabs;
    [SerializeField] private GameObject[] slideObstaclePrefabs;
    [SerializeField] private GameObject[] slideOrJumpObstaclePrefabs;

    [Header("Power-Up Prefabs")]
    [SerializeField] private GameObject[] powerUpPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] obstacleSpawnPoints;
    [SerializeField] private Transform[] powerUpLanePoints;

    [Header("Obstacle Type Weights (0–1)")]
    [SerializeField][Range(0f, 1f)] private float vehicleWeight = 0.5f;
    [SerializeField][Range(0f, 1f)] private float jumpObstacleWeight = 0.25f;
    // slideObstacleWeight is the remainder

    [Header("Power-Up Settings")]
    [SerializeField][Range(0f, 1f)] private float powerUpSpawnChance = 0.65f;
    [SerializeField] private bool guaranteeFirstTile = false;

    private void Start()
    {
        PopulateObstacles();
        TrySpawnPowerUp();
    }

    private void PopulateObstacles()
    {
        foreach (Transform spawnPoint in obstacleSpawnPoints)
        {
            if (spawnPoint == null) continue;

            GameObject chosen = PickObstaclePrefab();
            if (chosen != null)
                Instantiate(chosen, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        }
    }

    private GameObject PickObstaclePrefab()
    {
        float roll = Random.value;

        if (roll < vehicleWeight)
        {
            if (vehiclePrefabs != null && vehiclePrefabs.Length > 0)
                return vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        }
        else if (roll < vehicleWeight + jumpObstacleWeight)
        {
            if (jumpObstaclePrefabs != null && jumpObstaclePrefabs.Length > 0)
                return jumpObstaclePrefabs[Random.Range(0, jumpObstaclePrefabs.Length)];
        }
        else
        {
            bool useSlideOnly = Random.value < 0.5f;

            if (useSlideOnly)
            {
                if (slideObstaclePrefabs != null && slideObstaclePrefabs.Length > 0)
                    return slideObstaclePrefabs[Random.Range(0, slideObstaclePrefabs.Length)];
            }
            else
            {
                if (slideOrJumpObstaclePrefabs != null && slideOrJumpObstaclePrefabs.Length > 0)
                    return slideOrJumpObstaclePrefabs[Random.Range(0, slideOrJumpObstaclePrefabs.Length)];
            }
        }

        if (vehiclePrefabs != null && vehiclePrefabs.Length > 0)
            return vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];

        Debug.LogWarning($"TilePopulator on {name}: no obstacle prefabs assigned for rolled type.");
        return null;
    }

    private void TrySpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        if (powerUpLanePoints == null || powerUpLanePoints.Length == 0) return;

        bool shouldSpawn = guaranteeFirstTile || Random.value <= powerUpSpawnChance;
        if (!shouldSpawn) return;

        Transform lane = powerUpLanePoints[Random.Range(0, powerUpLanePoints.Length)];
        GameObject powerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Instantiate(powerUp, lane.position, lane.rotation, lane);
    }
}