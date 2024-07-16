using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // SerializedFields---
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private LayerMask wallLayer;
    // Fields----
    private bool hasDetectedARoom;


    // Properties----
    public bool HasDetectedARoom => hasDetectedARoom;

    // Unity Events----

    // Public Methods----
    public bool CheckIfDetectsARoom ()
    {
        hasDetectedARoom = Physics.CheckSphere(transform.position - new Vector3(0, 0, 1), 0.5f, roomLayer.value);
        return hasDetectedARoom;
    }

    // Private Methods----
}
