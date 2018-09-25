using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIControl : MonoBehaviour
{

    private float accuracy = 1.75f;
    private float idleDuration = 0.25f;
    private float stopTime;
    private float firstSpeed;
    private float speedFact;

    private float threatDetectionRadius = 10f;
    private float fleeRadius = 10f;
    private bool isFleeing;
    private float randomFleeTime = -1; //keeps Time.time when agent can't find a way to flee

    private List<Transform> waypoints;

    NavMeshAgent agent;
    Animator anim;

    private void Awake()
    {
        waypoints = GameObject.FindGameObjectsWithTag("WayPoint")
                              .OrderBy(wp => wp.name)
                              .Select(wp => wp.transform)
                              .ToList();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        firstSpeed = agent.speed;

        //for to start walking at a random time 
        stopTime = Time.time + (Random.Range(0.1f, 3f) * 2);

        //PickDestination();
    }

    private void Update()
    {
        HasWayToMove = (agent.pathEndPosition - transform.position).magnitude > accuracy;

        if (randomFleeTime > 0 && Time.time > randomFleeTime + 0.5f)
        {
            DetectThreatAndFlee(threatPosition);
            randomFleeTime = -1;
        }
        else if (!HasWayToMove)// && (isFleeing || Time.time > stopTime + idleDuration))
        {
            PickDestination();
        }
    }


    public bool HasWayToMove
    {
        get { return _isNearToPathEnd; }
        set
        {
            if (_isNearToPathEnd != value)
            {
                _isNearToPathEnd = value;
                if (_isNearToPathEnd)
                {
                    if (!isFleeing)
                    {
                        if (anim != null)
                            anim.SetFloat("walkOffset", Random.Range(0f, 1f));
                        SetWalking();
                    }
                }
                else
                {
                    if (anim != null)
                        anim.SetTrigger("isIdle");
                    stopTime = Time.time;
                    if (isFleeing)
                        agent.ResetPath();
                    isFleeing = false;
                    randomFleeTime = -1;
                }
            }
        }
    }
    private bool _isNearToPathEnd;


    public void DetectThreatAndFlee(Vector3 threat)
    {
        threatPosition = threat.With(y: transform.position.y);

        if (transform.DistanceTo(threatPosition.With(y: transform.position.y)) < threatDetectionRadius)
        {
            fleeDirection = (transform.position - threatPosition).normalized;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fleeDirection), 0.5f);
            FleeFromThreat(fleeRadius);
        }
    }

    private void FleeFromThreat(float fleeDistance)
    {
        fleeDirection = (transform.position - threatPosition).normalized;
        fleeGoal = transform.position + (fleeDirection * fleeDistance);

        fleePath = new NavMeshPath();
        agent.CalculatePath(fleeGoal, fleePath);

        if (fleePath.status != NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(fleePath.corners[fleePath.corners.Length - 1]);
            isFleeing = true;
        }
        else if (NavMesh.SamplePosition(fleeDirection, out navHit, fleeDistance, 1))
        {
            fleeGoal = navHit.position;
            agent.SetDestination(fleeGoal);
            isFleeing = true;
        }
        else if (fleeDistance < fleeRadius * 0.1f) //descriptions are at else block
        {
            int i = 0;
            //try to find a random fleeDirection
            do
            {
                fleeDirection = (Random.insideUnitSphere * fleeDistance * 0.5f) + transform.position;
                isFleeing = NavMesh.SamplePosition(fleeDirection, out navHit, fleeDistance, 1);
                randomFleeTime = Time.time;
                i++; //don't push too hard
            } while (!isFleeing || i < 30);

            if (isFleeing)
                agent.SetDestination(navHit.position);
        }
        else
        {   //We are trying half distance.
            FleeFromThreat(fleeDistance * 0.5f);
            //At fourth time fleeDistance will be fleeRadius * 0.125f
            //At fifth previous block will run


            randomFleeTime = Time.time;//for trying to find a better way at every 500ms
        }

        if (isFleeing)
        {
            if (anim != null)
                anim.SetTrigger("isRunning");
            agent.speed = 10;
            agent.angularSpeed = 500;
        }
    }
    private NavMeshHit navHit;
    private Vector3 fleeDirection;
    private Vector3 fleeGoal;
    private NavMeshPath fleePath;
    private Vector3 threatPosition;

    private void PickDestination()
    {
        //currentGoal = goalLocations[Random.Range(0, goalLocations.Length)];

        orderedWPs = waypoints
            .Where(g => g != currentGoal && g != prevWP && g != beforePrevWP)
            .OrderBy(g => g.DistanceTo(transform.position));

        tmpPrev = currentGoal;
        currentGoal = orderedWPs.FirstOrDefault();
        beforePrevWP = prevWP;
        prevWP = tmpPrev;

        agent.SetDestination(currentGoal.transform.position);
    }
    private IOrderedEnumerable<Transform> orderedWPs;
    private Transform currentGoal;
    private Transform prevWP;
    private Transform beforePrevWP;
    private Transform tmpPrev; //To keep next prevWP until query to be executed;

    private void ResetAgent()
    {
        SetWalking();
        agent.ResetPath();
    }

    private void SetWalking()
    {
        speedFact = Random.Range(0.75f, 1.5f);
        agent.speed = firstSpeed * speedFact;
        agent.angularSpeed = 120;
        if (anim != null)
        {
            anim.SetFloat("speedFact", speedFact);
            anim.SetTrigger("isWalking");
        }
    }
}
