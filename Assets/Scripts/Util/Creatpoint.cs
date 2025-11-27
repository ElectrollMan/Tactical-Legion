using System.Collections.Generic;
using UnityEngine;
using TurnBaseUtil;

public class Creatpoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> gameObjects = new List<GameObject>();
    public GameObject playerTeam1;
    public GameObject playerTeam2;
    public int teamNumber = 3;

    void Start()
    {
        int requiredSpawnCount = teamNumber * 2;
        if (gameObjects == null || gameObjects.Count < requiredSpawnCount)
        {
            Debug.LogError($"Not enough spawn points! Need at least {requiredSpawnCount}, but found {gameObjects.Count}");
            return;
        }

        List<GameObject> shuffledSpawns = new List<GameObject>(gameObjects);
        ShuffleList(shuffledSpawns);
        List<GameObject> chosenSpawns = shuffledSpawns.GetRange(0, requiredSpawnCount);

        for (int i = 0; i < teamNumber; i++)
        {
            int indexA = i * 2;
            int indexB = i * 2 + 1;

            GameObject spawnA = chosenSpawns[indexA];
            GameObject spawnB = chosenSpawns[indexB];

            GameObject player1 = Instantiate(playerTeam1, spawnA.transform.position, spawnA.transform.rotation);
            player1.GetComponent<TeamPlayer>().Copy(Global.teamA.teamPlayers[i]);
            GameManager.Instance.teamA_Players.Add(player1);

            GameObject player2 = Instantiate(playerTeam2, spawnB.transform.position, spawnB.transform.rotation);
            player2.GetComponent<TeamPlayer>().Copy(Global.teamB.teamPlayers[i]);
            GameManager.Instance.teamB_Players.Add(player2);
        }

        Debug.Log($"Spawned {requiredSpawnCount} players using {chosenSpawns.Count} random spawn points from {gameObjects.Count} total.");
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
