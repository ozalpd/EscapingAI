using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A simple navigation agent controller. 2018/08/25 Özalp
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentController : MonoBehaviour
{
    [Header("Navigation Agent")]
    [Tooltip("Update Interval for navigation path calculations. Smaller values need more CPU power.")]
    public float updateInterval = 0.5f;

    [Tooltip("Minimum distance between target's current position and last position at update time. Smaller values need more CPU power.")]
    public float updateDistance = 0.5f;

    private float lastUpdateTime = 0;

    //Target's last position at update time.
    private Vector3 targetAtUpdate = Vector3.zero;

    private NavMeshAgent agent;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("No NavMeshAgent found!");
    }

    protected virtual void Update()
    {
        CheckAndUpdateDestination();
    }

    /// <summary>
    /// Target transform. When set to null, sets agent's destination to its position.
    /// </summary>
    /// <value>The target.</value>
    public Transform Target
    {
        get { return _target; }
        set
        {
            _target = value;
            UpdateDestination();
        }
    }
    private Transform _target;


    protected virtual void CheckAndUpdateDestination()
    {
        if (Target == null)
            return;

        if ((lastUpdateTime + updateInterval) > Time.time)
            return;

        if (Target.DistanceTo(targetAtUpdate) < updateDistance) //did our target move enough
            return;

        UpdateDestination();
    }

    protected virtual void UpdateDestination()
    {
        if (Target == null)
        {
            agent.destination = transform.position; //keep same position
            targetAtUpdate = Vector3.zero;
        }
        else
        {
            agent.destination = Target.position;
            targetAtUpdate = Target.position;
        }

        lastUpdateTime = Time.time;
    }
}
