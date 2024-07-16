using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    // SerielizedFields----
    [Header("References")]
    [SerializeField] private Vector2Int[] possiblesLengthsAndWidths;
    
    [SerializeField] private Transform[] leftAndRightWallsPositions;
    [SerializeField] private Transform[] upAndDownWallsPositions;
    [SerializeField] private Transform[] floorAndRoofPositions;

    [SerializeField] private LayerMask roomLayer;
    
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject floor;

    // Fields----
    private Vector2Int currentLenghtAndWidth;

    int highestValue = 0;
    private List<GameObject> generatedLeftAndRightWalls = new List<GameObject>();
    private List<GameObject> generatedUpAndDonWalls = new List<GameObject>();
    private List<GameObject> generatedFloors = new List<GameObject>();
    private List<GameObject> generatedRoofs = new List<GameObject>();
    private bool insideOtherRoom;
    private Collider[] detectedRooms;

    // Properties----
    public bool InsideOtherRoom => insideOtherRoom;

    // Unity Events-----
    // private void OnTriggerEnter (Collider other)
    // {
    //     if (other.gameObject.layer == gameObject.layer && other != gameObject)
    //     {
    //         insideOtherRoom = true;
    //     }
    // }
    // private void OnTriggerExit (Collider other)
    // {
    //     if (other.gameObject.layer == gameObject.layer && other != gameObject)
    //     {
    //         insideOtherRoom = false;
    //     }
    // }

    // Public Methods----
    public void InitRoom (Vector2Int minAndMAxRandom)
    {
        CheckHighestValueInLenghtAndWidth(); 
        SetRoomScale(minAndMAxRandom);
    }

    public void RescaleRoom ()
    {
        ResetRoom();
        SetRoomScale(Vector2Int.zero);
    }

    public void GenerateRoom ()
    {
        ActivateAndSetWalls();
    }

    public bool CheckIfIsInsideOfanotherRoom ()
    {
        detectedRooms = null;
        detectedRooms = Physics.OverlapBox(transform.position, transform.localScale * 1.5f, transform.rotation, roomLayer.value);

        if (detectedRooms.Length >= 1)
            if (detectedRooms.Length == 1 && detectedRooms[0].gameObject != gameObject || detectedRooms.Length > 1)
            {
                insideOtherRoom = true;
                return insideOtherRoom;
            }
            
        insideOtherRoom = false;
        return insideOtherRoom;
    }

    private void OnDrawGizmos ()
    {
        Gizmos.DrawCube(transform.localPosition, transform.localScale * 1.5f);
    }

    public void CheckNeighborRooms()
    {
        foreach (GameObject wall in generatedLeftAndRightWalls)
        {
            if (wall.TryGetComponent<Wall>(out Wall wallSC)) wallSC.CheckIfDetectsARoom();
        }
    }

    public void ResetRoom ()
    {
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;
        foreach (GameObject wall in generatedLeftAndRightWalls)
        {
            wall.transform.localScale = new Vector3(1, wall.transform.localScale.y, 1);           
            wall.SetActive(false);
        }
        foreach (GameObject wall in generatedUpAndDonWalls)
        {
            wall.transform.localScale = new Vector3(1, wall.transform.localScale.y, 1);
            wall.SetActive(false);
        }
    }
    
    // Private Methods----

    private void CheckHighestValueInLenghtAndWidth ()
    {
        for (int i = 0; i < possiblesLengthsAndWidths.Length; i++)
        {
            if (highestValue < possiblesLengthsAndWidths[i].x) highestValue = possiblesLengthsAndWidths[i].x;
            else continue;
        }
        for (int i = 0; i < possiblesLengthsAndWidths.Length; i++)
        {
            if (highestValue < possiblesLengthsAndWidths[i].y) highestValue = possiblesLengthsAndWidths[i].y;
            else continue;
        }
        GenerateWallsAsHighestValue();
    }

    private void GenerateWallsAsHighestValue ()
    {
        foreach (Transform wallPos in leftAndRightWallsPositions)
        {
            for (int i = 0; i < highestValue; i++)
            {
                generatedLeftAndRightWalls.Add(Instantiate(wall, wallPos.position, wallPos.rotation));
                generatedLeftAndRightWalls[^1].transform.SetParent(wallPos);
                generatedLeftAndRightWalls[^1].SetActive(false);
            }
        }
        foreach (Transform wallPos in upAndDownWallsPositions)
        {
            for (int i = 0; i < highestValue; i++)
            {
                generatedUpAndDonWalls.Add(Instantiate(wall, wallPos.position, wallPos.rotation));
                generatedUpAndDonWalls[^1].transform.SetParent(wallPos);
                generatedUpAndDonWalls[^1].SetActive(false);
            }
        }
    }
    
    private void SetRoomScale (Vector2Int minAndMAxRandom)
    {
        if(minAndMAxRandom == Vector2.zero) currentLenghtAndWidth = possiblesLengthsAndWidths[Random.Range(0, possiblesLengthsAndWidths.Length)];
        else currentLenghtAndWidth = possiblesLengthsAndWidths[Random.Range(minAndMAxRandom.x, minAndMAxRandom.y)];
        transform.localScale = new Vector3(currentLenghtAndWidth.x, transform.localScale.y, currentLenghtAndWidth.y);
    }

    public void ActivateAndSetWalls ()
    {
        float currentXScale = transform.localScale.x;
        float currentZScale = transform.localScale.z;

        int index = 0;
        float wallScale = generatedLeftAndRightWalls[0].transform.localScale.x / currentXScale;
        foreach (Transform sideWallTransform in leftAndRightWallsPositions)
        {
            Vector3 currentWallPosition;
            if (currentXScale == 1) currentWallPosition = Vector3.zero;
            else currentWallPosition = new Vector3((sideWallTransform.localScale.x - wallScale) *-1, 0, 0);
            
            for (int i = 0; i < (sideWallTransform.localScale.x / wallScale) ; i++)
            {
                generatedLeftAndRightWalls[index].transform.localScale = new Vector3(wallScale,
                    generatedLeftAndRightWalls[index].transform.localScale.y, 
                    sideWallTransform.localScale.z / currentZScale); 
                
                generatedLeftAndRightWalls[index].gameObject.transform.localPosition =
                    currentWallPosition;
                
                generatedLeftAndRightWalls[index].gameObject.transform.rotation = sideWallTransform.rotation;
                generatedLeftAndRightWalls[index].gameObject.SetActive(true);
                
                currentWallPosition.x += wallScale *2;
                index++;
            }

            index += highestValue - index;
        }
        index = 0;
        wallScale = generatedUpAndDonWalls[0].transform.localScale.x / currentZScale;
        foreach (Transform sideWallTransform in upAndDownWallsPositions)
        {
            Vector3 currentWallPosition;
            if (currentZScale == 1) currentWallPosition = Vector3.zero;
            else currentWallPosition = new Vector3((sideWallTransform.localScale.x - wallScale) *-1, 0, 0);
            for (int i = 0; i < (sideWallTransform.localScale.x / wallScale) ; i++)
            {
                generatedUpAndDonWalls[index].transform.localScale = new Vector3(wallScale,
                    generatedUpAndDonWalls[index].transform.localScale.y, 
                    sideWallTransform.localScale.z / currentXScale); 
                
                generatedUpAndDonWalls[index].gameObject.transform.localPosition =
                    currentWallPosition;
                
                generatedUpAndDonWalls[index].gameObject.transform.rotation = sideWallTransform.rotation;
                generatedUpAndDonWalls[index].gameObject.SetActive(true);
                
                currentWallPosition.x += wallScale *2;
                index++;
            }
            index += highestValue - index;
        }
    }

    private void ActivateAndSetRoofAndFloor ()
    {
        
    }
    
    //-------------------------------------------------------
    
    
    private void GenerateFloorAndRoof ()
    {
        float xScale = transform.localScale.x;
        float yScale = transform.localScale.z;

        float xPos;
        float zPos;
        
        if (xScale == 1) xPos = 0;
        else if (xScale % 2 != 0) xPos = -xScale + wall.transform.localScale.x;
        else xPos= -xScale - (wall.transform.localScale.x *-1);
        
        if (yScale == 1) zPos = 0;
        else if (yScale % 2 != 0) zPos = -yScale + wall.transform.localScale.z;
        else zPos = -yScale - (wall.transform.localScale.z *-1);
        
        foreach (Transform floorPos in floorAndRoofPositions)
        {
            for (float i = xPos; i < currentLenghtAndWidth.x; i += xScale)
            {
                for (float j = zPos; j < currentLenghtAndWidth.y; j += yScale)
                {
                    generatedFloors.Add(Instantiate(floor, new Vector3(i, floorPos.position.y, j), floorPos.rotation));
                    generatedFloors[^1].transform.SetParent(floorPos);
                }
            }
        }
        
    }
}
