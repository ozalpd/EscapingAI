using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationTypes
{
    public static bool UsesRigidbody(this PlayerMoveMechanism moveMechanism)
    {
        return moveMechanism == PlayerMoveMechanism.AddForce
            || moveMechanism == PlayerMoveMechanism.SetVelocity;
    }
}
