using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : FollowReference
{
    [Header("Player")]
    [Tooltip("A reference to player in the scene.")]
    public AbstractPlayerController player;
    [Tooltip("Offset multiplier for player's move direction.")]
    public float moveOffset = 4f;

    [Tooltip("Camera speed when player stopped.")]
    public float idleSpeed = 3f;

    private Vector3 originalOffset = Vector3.zero;
    private float originalSpeed = 5f;


    protected override void Awake()
    {
        if (player == null)
            player = FindObjectOfType<AbstractPlayerController>();

        if (player != null)
        {
            base.reference = player.transform;
        }
        else
        {
            Debug.LogError("No player found. CameraController needs a player reference!");
        }

        base.Awake();

        originalOffset = base.offset;
        originalSpeed = base.speed;
        GameManager.OnPlayerMoving += GameController_OnPlayerMoving;
        GameManager.OnPlayerStopped += GameController_OnPlayerStopped;
        base.useLateUpdate = true;
    }

    private void GameController_OnPlayerMoving(AbstractPlayerController playerController, Vector3 movement)
    {
        if (Time.time > followTime)
        {
            offset = originalOffset + (moveOffset * movement.normalized);
            speed = originalSpeed;
        }
    }

    private void GameController_OnPlayerStopped(AbstractPlayerController playerController, Vector3 movement)
    {
        speed = idleSpeed;
        offset = originalOffset;
    }
}
