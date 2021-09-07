using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject playerPrefab; // player prefab to be spawned
    void Awake()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation); // spawns player
    }
}
