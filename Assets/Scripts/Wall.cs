using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // SerializedFields---
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject door;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private LayerMask wallLayer;
    // Fields----
    private Collider currentCollider;
    private Collider[] detectedWalls = Array.Empty<Collider>();
    private Collider[] detectedRooms = Array.Empty<Collider>();
    private RoomGenerator roomParent;
    private RoomGenerator roomDetected;
    private GameObject wallDetected;

    // Properties----
    public RoomGenerator RoomParent => roomParent;
    
    //public bool HasDetectedARoom => hasDetectedARoom;

    // Unity Events----

    private void Start ()
    {
        currentCollider = GetComponent<Collider>();
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.K))print( CheckIfDetectsARoom());
    }

    // Public Methods----
    public void GetRoomParent (RoomGenerator recivedRoom)
    {
        roomParent = recivedRoom;
    }
    public RoomGenerator CheckIfDetectsARoom ()
    {
        if (!gameObject.activeInHierarchy) return null;

        List<Collider> detectedWallsList = new List<Collider>();
        detectedWalls = Physics.OverlapSphere(transform.position + Vector3.up,0.1f, wallLayer.value);
        detectedWallsList = detectedWalls.ToList();
        
        if(detectedWallsList.Contains(currentCollider)) detectedWallsList.Remove(currentCollider);
        
        foreach (Collider wall in detectedWalls)
        {
            if (wall.gameObject.transform.position == transform.position && wall != currentCollider)
            {
                wallDetected = wall.gameObject;
                
                List<Collider> detectedRoomsList = new List<Collider>();
                detectedRooms = Physics.OverlapSphere(transform.position + Vector3.up,0.1f, roomLayer.value);
                detectedRoomsList = detectedRooms.ToList();

                if (detectedRoomsList.Contains(roomParent.RoomCollider)) detectedRoomsList.Remove(roomParent.RoomCollider);
                if(detectedRoomsList.Count > 0) roomDetected = detectedRoomsList[0].gameObject.GetComponent<RoomGenerator>();
            }
        }
        return detectedRooms.Length >= 1 ? roomDetected : null;
    }

    public void ChangeWallToDoor ()
    {
        wall.SetActive(false);
        wallDetected.SetActive(false);
        door.SetActive(true);
    }

    private void OnDrawGizmos ()
    {
        Gizmos.DrawSphere(transform.position+ Vector3.up, 0.1f);
    }

    // Private Methods----
}
