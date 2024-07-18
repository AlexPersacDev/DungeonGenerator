using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // SerializedFields---
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private LayerMask wallLayer;
    // Fields----
    private Collider[] hasDetectedARoom;


    // Properties----
    //public bool HasDetectedARoom => hasDetectedARoom;

    // Unity Events----

    // Public Methods----
    public void CheckIfDetectsARoom ()
    {
        hasDetectedARoom = Physics.OverlapSphere(transform.position - new Vector3(0, 0, 1), 0.5f, roomLayer.value);
        hasDetectedARoom[0].gameObject.SetActive(false);
    }

    // Private Methods----
}
