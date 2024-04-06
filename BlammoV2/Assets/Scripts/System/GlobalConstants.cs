using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants : MonoBehaviour
{
    public LayerMask GeneralWorldMask;
    public static GlobalConstants Instance;

    public void Awake()
    {
        Instance = this;
    }
}