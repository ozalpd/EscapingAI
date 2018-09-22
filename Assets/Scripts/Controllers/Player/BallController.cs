using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : AbstractPlayerController
{
    [Header("Ball")]
    [Tooltip("Force that is constantly added to ball's rigidbody.")]
    [Range(-10f, 10f)]
    public float constAcceleration = 1f;

    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        moveMechanism = PlayerMoveMechanism.AddForce;
    }

    public override bool Shielding { get; set; }

    public override void GetIdle() { }

    public override void Move(Vector3 movement)
    {
        movement.z += constAcceleration * speed;
        rb.AddForce(movement);
    }

    public override void ResetValues() { }
}
