using System.Collections.Generic;
using UnityEngine;

public class TilePopulator : MonoBehaviour
{

    #region Inspector Fields
    [Header("Special Tile Settings")]
    [SerializeField] private bool allowPowerUpsOnBlockedLanes = false;

    [Header("Vehicle Prefabs")]
    [SerializeField] private GameObject[] vehiclePrefabs;

    [Header("Obstacle Type Prefabs")]
    [SerializeField] private GameObject[] jumpObstaclePrefabs;
    [SerializeField] private GameObject[] slideObstaclePrefabs;
    [SerializeField] private GameObject[] slideOrJumpObstaclePrefabs;

    [Header("Power-Up Prefabs")]
    [SerializeField] private GameObject[] powerUpPrefabs;

    [Header("Coin Prefab")]
    [SerializeField] private GameObject coinPrefab;

    [Header("Spawn Points  (lane = index % 3 → 0 left | 1 centre | 2 right)")]
    [SerializeField] private Transform[] obstacleSpawnPoints;
    [SerializeField] private Transform[] powerUpLanePoints;
    [SerializeField] private Transform[] coinSpawnPoints;

    [Header("Obstacle Type Weights  (should sum to <= 1)")]
    [SerializeField][Range(0f, 1f)] private float vehicleWeight = 0.70f;
    [SerializeField][Range(0f, 1f)] private float jumpObstacleWeight = 0.15f;


    [Header("Spawn Chances")]
    [SerializeField][Range(0f, 1f)] private float powerUpChance = 0.10f;
    [SerializeField][Range(0f, 1f)] private float coinChance = 0.80f;

    #endregion

    #region Constants

    private const int Lanes = 3;

    #endregion


    private void Start()
    {
        bool[] laneBlocked = GetBlockedLanes();
        SpawnObstacles(laneBlocked);
        SpawnPickups(laneBlocked);
    }

    #region Lane Analysis

    private bool[] GetBlockedLanes()
    {
        bool[] blocked = new bool[Lanes];

        if (obstacleSpawnPoints == null) return blocked;

        for (int i = 0; i < obstacleSpawnPoints.Length; i++)
        {
            if (obstacleSpawnPoints[i] != null)
                blocked[i % Lanes] = true;
        }

        return blocked;
    }

    #endregion
    #region Obstacle Spawning

    private void SpawnObstacles(bool[] laneBlocked)
    {
        if (obstacleSpawnPoints == null) return;

        bool bothOuterLanesBlocked = laneBlocked[0] && laneBlocked[2];
        bool allThreeLanesBlocked = bothOuterLanesBlocked && laneBlocked[1];

        var prefabs = new GameObject[Lanes];
        var allPools = new List<Transform>[Lanes];
        var middlePools = new List<Transform>[Lanes];
        bool anyVehicle = false;

        for (int lane = 0; lane < Lanes; lane++)
        {
            if (!laneBlocked[lane]) continue;

            allPools[lane] = new List<Transform>();
            middlePools[lane] = new List<Transform>();

            for (int i = lane; i < obstacleSpawnPoints.Length; i += Lanes)
            {
                Transform point = obstacleSpawnPoints[i];
                if (point == null) continue;

                allPools[lane].Add(point);
                if (i / Lanes == 1) middlePools[lane].Add(point);
            }

            if (allPools[lane].Count == 0) continue;

            bool forceVehicle = bothOuterLanesBlocked && (lane == 0 || lane == 2);
            bool forceReaction = allThreeLanesBlocked && lane == 1;

            if (forceVehicle) prefabs[lane] = PickVehicle();
            else if (forceReaction) prefabs[lane] = PickReactionObstacle();
            else prefabs[lane] = PickObstaclePrefab();

            if (prefabs[lane] != null && !IsJumpOrSlide(prefabs[lane]))
                anyVehicle = true;
        }

        for (int lane = 0; lane < Lanes; lane++)
        {
            if (!laneBlocked[lane] || allPools[lane] == null || allPools[lane].Count == 0) continue;

            bool protectedCentre = allThreeLanesBlocked && lane == 1;

            if (anyVehicle && IsJumpOrSlide(prefabs[lane]) && !protectedCentre)
                prefabs[lane] = PickVehicle();

            bool isReaction = IsJumpOrSlide(prefabs[lane]);
            List<Transform> pool = (isReaction && middlePools[lane].Count > 0)
                ? middlePools[lane]
                : allPools[lane];

            SpawnAt(prefabs[lane], pool[Random.Range(0, pool.Count)]);
        }
    }

    private bool IsJumpOrSlide(GameObject prefab)
    {
        if (prefab == null) return false;

        foreach (GameObject p in jumpObstaclePrefabs)
            if (p == prefab) return true;

        foreach (GameObject p in slideObstaclePrefabs)
            if (p == prefab) return true;

        foreach (GameObject p in slideOrJumpObstaclePrefabs)
            if (p == prefab) return true;

        return false;
    }

    #endregion

    private void SpawnPickups(bool[] laneBlocked)
    {
        for (int lane = 0; lane < Lanes; lane++)
        {
            bool blocked = laneBlocked[lane];

            if (blocked && !allowPowerUpsOnBlockedLanes) continue;

            float roll = Random.value;

            if (roll < powerUpChance)
            {
                var candidates = new List<Transform>();
                for (int i = lane; i < powerUpLanePoints.Length; i += Lanes)
                    if (powerUpLanePoints[i] != null)
                        candidates.Add(powerUpLanePoints[i]);

                if (candidates.Count > 0)
                    SpawnAt(PickRandom(powerUpPrefabs), candidates[Random.Range(0, candidates.Count)]);
            }
            else if (!blocked && roll < powerUpChance + coinChance)
            {
                for (int i = lane; i < coinSpawnPoints.Length; i += Lanes)
                    SpawnAt(coinPrefab, coinSpawnPoints[i]);
            }
        }
    }

    #region Prefab Selection
    private GameObject PickVehicle() => PickRandom(vehiclePrefabs);

    private GameObject PickObstaclePrefab()
    {
        float roll = Random.value;

        if (roll < vehicleWeight)
            return PickRandomWithFallback(vehiclePrefabs);

        if (roll < vehicleWeight + jumpObstacleWeight)
            return PickRandomWithFallback(jumpObstaclePrefabs);

        return Random.value < 0.5f
            ? PickRandomWithFallback(slideObstaclePrefabs)
            : PickRandomWithFallback(slideOrJumpObstaclePrefabs);
    }

    private static GameObject PickRandom(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
    }

    private GameObject PickRandomWithFallback(GameObject[] array)
    {
        if (array != null && array.Length > 0)
            return array[Random.Range(0, array.Length)];

        Debug.LogWarning($"TilePopulator on '{name}': obstacle array empty — falling back to vehicle.");
        return PickRandom(vehiclePrefabs);
    }

    #endregion

    private void SpawnAt(GameObject prefab, Transform point, bool usePointRotation = false)
    {
        if (prefab == null || point == null) return;

        Vector3 position = new Vector3(
            point.position.x,
            point.position.y + prefab.transform.position.y,
            point.position.z
        );

        Quaternion rotation = usePointRotation ? point.rotation : prefab.transform.rotation;

        Instantiate(prefab, position, rotation, point);
    }

    private GameObject PickReactionObstacle()
    {
        float roll = Random.value;
        if (roll < 1f / 3f) return PickRandomWithFallback(jumpObstaclePrefabs);
        if (roll < 2f / 3f) return PickRandomWithFallback(slideObstaclePrefabs);
        return PickRandomWithFallback(slideOrJumpObstaclePrefabs);
    }
}