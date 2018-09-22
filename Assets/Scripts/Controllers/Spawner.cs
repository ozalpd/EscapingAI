using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Reference objects to be spawn")]
    public GameObject[] reference;
    private int[] _referenceId;

    [Tooltip("If this is unchecked spawns references sequentally.")]
    public bool spawnRandomly = true;

    public Transform[] spawnPoints;


    public int maxObjectsOnScreen = 20;
    public int totalObjectsToSpawn = 100;
    public float minSpawnTime = 0.25f;
    public float maxSpawnTime = 2f;
    public int objectsPerSpawn = 5;

    [Header("AI or NavMeshAgent Controller")]
    [Tooltip("NavMeshAgent Controller's target")]
    public Transform aITarget;

    [Tooltip("Spawn as headed to AI Target")]
    public bool spawnHeadedToTarget = false;

    private int spawnedObjects = 0;
    private float nextSpawnTime = 0;
    private float currentSpawnTime = 0;

    private void Awake()
    {
        _referenceId = new int[reference.Length];
        for (int i = 0; i < reference.Length; i++)
        {
            _referenceId[i] = reference[i].GetInstanceID();
            ObjectPool.GetOrInitPool(reference[i]);
        }
    }

    private void Update()
    {
        currentSpawnTime += Time.deltaTime;
        if (currentSpawnTime > nextSpawnTime)
        {
            currentSpawnTime = 0;
            nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            if (objectsPerSpawn > 0 && spawnedObjects < totalObjectsToSpawn)
            {
                List<int> previousSpawnLocations = new List<int>();
                if (objectsPerSpawn > spawnPoints.Length)
                {
                    objectsPerSpawn = spawnPoints.Length - 1;
                }

                objectsPerSpawn = (objectsPerSpawn > totalObjectsToSpawn) ? objectsPerSpawn - totalObjectsToSpawn : objectsPerSpawn;

                for (int i = 0; i < objectsPerSpawn; i++)
                {
                    if (spawnedObjects < maxObjectsOnScreen)
                    {
                        spawnedObjects += 1;
                        // 1
                        int spawnPoint = -1;
                        // 2
                        while (spawnPoint == -1)
                        {
                            // 3
                            int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                            // 4
                            if (!previousSpawnLocations.Contains(randomNumber))
                            {
                                previousSpawnLocations.Add(randomNumber);
                                spawnPoint = randomNumber;
                            }
                        }

                        var spawnLocation = spawnPoints[spawnPoint];
                        var go = ObjectPool.GetInstance(_referenceId[GetNextIndex()], spawnLocation.position, Quaternion.identity);

                        if (aITarget != null)
                        {
                            var agentController = go.GetComponent<NavAgentController>();
                            if (agentController != null)
                                agentController.Target = aITarget.transform;
                        }

                        if (spawnHeadedToTarget)
                            go.transform.LookAt(new Vector3(aITarget.transform.position.x,
                                                                  go.transform.position.y,
                                                                  aITarget.transform.position.z));
                    }
                }
            }
        }
    }


    private int GetNextIndex()
    {
        if (reference == null || reference.Length < 1) //if reference is null or length is zero let the compiler throws a null reference exception
        {
            return 0;
        }
        else if (spawnRandomly)
        {
            return Random.Range(0, reference.Length - 1);
        }
        else
        {
            index = index.HasValue && index.Value < reference.Length ? index.Value + 1 : 0;
            return index.Value;
        }
    }

    private int? index;
}
