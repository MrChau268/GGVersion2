using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;  // drag your FBX prefab here
    public Transform spawnPoint;      // drag an empty GameObject for position

    void Start()
    {
        Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}