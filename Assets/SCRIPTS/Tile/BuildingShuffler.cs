using System.Collections.Generic;
using UnityEngine;

public class BuildingShuffler : MonoBehaviour
{
    private List<Transform> buildingPositions = new List<Transform>();
    private List<GameObject> buildings = new List<GameObject>();

    void Start()
    {
        FindBuildingsAndPositions();
        ShuffleBuildings();
    }

    private void FindBuildingsAndPositions()
    {
        buildingPositions.Clear();
        buildings.Clear();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("BuildingPosition"))
            {
                buildingPositions.Add(child);
            }
            else if (child.CompareTag("Building"))
            {
                buildings.Add(child.gameObject);
            }
        }

        if (buildings.Count != buildingPositions.Count)
        {
            Debug.LogError("Mismatch: " + buildings.Count + " buildings, " + buildingPositions.Count + " positions.");
        }
    }

    public void ShuffleBuildings()
    {
        if (buildings.Count != buildingPositions.Count || buildings.Count == 0)
        {
            Debug.LogError("Error: Buildings and positions do not match or are missing.");
            return;
        }

        // Create a shuffled list of positions
        List<Transform> shuffledPositions = new List<Transform>(buildingPositions);
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPositions.Count);
            (shuffledPositions[i], shuffledPositions[randomIndex]) = (shuffledPositions[randomIndex], shuffledPositions[i]);
        }

        // Assign buildings to new positions and rotations
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].transform.SetPositionAndRotation(shuffledPositions[i].position, shuffledPositions[i].rotation);
        }
    }
}
