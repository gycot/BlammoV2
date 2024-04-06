using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    public static PhysicsController Instance;
    public float FallAddl = .75f;
    public float LowJumpAddl = .5f;
    public float GroundedFallAddl = 1f;
    public float FallSpeedFactor = .5f;

    public float GroundDistance = .05f;

    private void Awake()
    {
        Instance = this;
    }
}
