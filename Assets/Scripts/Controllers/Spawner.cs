using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Reference objects to be spawn")]
    public GameObject[] reference;
    private int[] refId;

    [Tooltip("If this is unchecked spawns references sequentally.")]
    public bool spawnRandomRef = true;

    public Transform[] spawnPositions;
    [Tooltip("If this is unchecked spawns into Spawn Points sequentally.")]
    public bool spawnRandomPos = true;

    [Tooltip("Number of objects to be spawn")]
    public int numToSpawn = 100;
    private int remain;
    [Tooltip("Shortest time between two spawns")]
    public float minSpawnTime = 0.25f;
    [Tooltip("Longest time between two spawns")]
    public float maxSpawnTime = 2f;

    [Tooltip("The player")]
    public Transform player;

    [Tooltip("Spawn as headed to AI Target")]
    public bool spawnHeadedToTarget = false;

    [Header("Animator")]
    [Range(1f, 10f)]
    public float delayIn = 3;
    [Range(1f, 10f)]
    public float delayOut = 3;

    private Animator animator;
    private int boolSpawningId;


    private void Awake()
    {
        refId = new int[reference.Length];
        for (int i = 0; i < reference.Length; i++)
        {
            refId[i] = reference[i].GetInstanceID();
            ObjectPool.GetOrInitPool(reference[i]);
        }

        animator = GetComponent<Animator>();
        boolSpawningId = Animator.StringToHash("Spawning");
    }

    private IEnumerator Start()
    {
        if (animator != null)
        {
            animator.SetBool(boolSpawningId, true);
            yield return new WaitForSeconds(delayIn);
        }

        if (reference == null || reference.Length < 1)
        {
            Debug.LogError("There is no reference prefabs attached!");
        }
        else
        {
            remain = numToSpawn;

            while (remain > 0)
            {
                var randPos = spawnPositions != null && spawnPositions.Length > 0
                            ? spawnPositions[Random.Range(0, spawnPositions.Length - 1)].position
                            : transform.position;
                var go = ObjectPool.GetInstance(refId[GetRefIndex()], randPos, new Quaternion(0, 0, 0, 0));
                if (player != null && spawnHeadedToTarget)
                    go.transform.LookAt(player);

                remain -= 1;

                yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            }
        }

        if (animator != null)
        {
            yield return new WaitForSeconds(delayOut);
            animator.SetBool(boolSpawningId, false);
        }

        gameObject.SetActive(false);
    }

    private Vector3 GetSpawnPoint()
    {
        if (spawnPositions == null || spawnPositions.Length < 1)
            return transform.position;

        if (spawnRandomPos)
            return spawnPositions[Random.Range(0, spawnPositions.Length - 1)].position;

        posIndex = posIndex.HasValue && posIndex.Value < spawnPositions.Length - 1 ? posIndex.Value + 1 : 0;
        return spawnPositions[posIndex.Value].position;
    }
    private int? posIndex;


    private int GetRefIndex()
    {
        if (spawnRandomRef)
        {
            return Random.Range(0, reference.Length - 1);
        }
        else
        {
            refIndex = refIndex.HasValue && refIndex.Value < reference.Length - 1 ? refIndex.Value + 1 : 0;
            return refIndex.Value;
        }
    }
    private int? refIndex;
}
