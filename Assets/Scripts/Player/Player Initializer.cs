using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    public GameObject playerPrefab; // Drag the player prefab here

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            // Instantiate the player if it doesn't exist
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            player.tag = "Player"; // Ensure the player has the correct tag
            DontDestroyOnLoad(player); // Make the player persist across scenes
        }
    }
}
