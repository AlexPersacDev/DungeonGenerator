using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    // SerializedFields---
    [SerializeField] private RoomGenerator room;
    [SerializeField] private int roomsCant;
    
    // Fields----
    private List<RoomGenerator> generatedRooms = new List<RoomGenerator>();
    private GameObject currentNeighbortRoom;
    
    // Properties----

    // Unity Events----
    private void Start ()
    {
        generatedRooms.Add(Instantiate(room, Vector3.zero, Quaternion.identity));
        generatedRooms[0].InitRoom(new Vector2Int(0, 1)); 
        generatedRooms[0].GenerateRoom(); 
        
        for (int i = 0; i < roomsCant; i++)
        {
            generatedRooms.Add(Instantiate(room, 
                Vector3.zero, 
                Quaternion.identity));
        }
        StartCoroutine(GenerateRooms());
    }
    // Public Methods----

    // Private Methods----
    private IEnumerator GenerateRooms ()
    {
        for (int i = 1; i <  generatedRooms.Count; i++)
        {
            currentNeighbortRoom = GetNeighbourRoom();

            generatedRooms[i].gameObject.SetActive(true);
            generatedRooms[i].transform.position = currentNeighbortRoom.transform.position;

            generatedRooms[i].InitRoom(Vector2Int.zero);
            
            PlaceRoomInNeighbourBounds(generatedRooms[i], currentNeighbortRoom);
            
            generatedRooms[i].GenerateRoom();
            yield return null;
            
            if (!generatedRooms[i].InsideOtherRoom) continue;
            
            while (generatedRooms[i].InsideOtherRoom)
            {
                generatedRooms[i].ResetRoom();
                yield return null;
            
                currentNeighbortRoom = GetNeighbourRoom();
                while (currentNeighbortRoom == generatedRooms[i].gameObject)
                {
                    currentNeighbortRoom = GetNeighbourRoom();
                }
                
                generatedRooms[i].RescaleRoom();
                
                generatedRooms[i].transform.localPosition = currentNeighbortRoom.transform.localPosition;
                
                PlaceRoomInNeighbourBounds(generatedRooms[i], currentNeighbortRoom);
                yield return null;
            }
            generatedRooms[i].GenerateRoom();
        }
        GenerateDoors();
        StopCoroutine(GenerateRooms());
    }

    private GameObject GetNeighbourRoom ()
    {
        return generatedRooms[Random.Range(0, generatedRooms.Count)].gameObject;
    }
    
    private void PlaceRoomInNeighbourBounds (RoomGenerator currentRoom, GameObject neighbourRoom)
    {
        if (GetRandomValue() < 0)
        {
            float zValue = neighbourRoom.transform.localScale.z + currentRoom.transform.localScale.z;
            
            currentRoom.transform.localPosition += new Vector3
            (CheckScaleRatio(currentRoom.transform.localScale.x, neighbourRoom.transform.localScale.x),
                transform.position.y, zValue);
        }

        else
        {
            float xValue = neighbourRoom.transform.localScale.x + currentRoom.transform.localScale.x;
            
            currentRoom.transform.localPosition += new Vector3(xValue, transform.position.y, 
                CheckScaleRatio(currentRoom.transform.localScale.z, 
                    neighbourRoom.transform.localScale.z));
        }
    }

    private float CheckScaleRatio (float currentRoomScale, float neighbourRoomScale)
    {
        float movements = 0;
        float highestScaleValue = currentRoomScale > neighbourRoomScale ? currentRoomScale : neighbourRoomScale;
        
        if (currentRoomScale % 2 == 0 && neighbourRoomScale % 2 == 0) // son pares
        {
            //se podrá mover tantas veces como maxScaleValue/2
            //return MoveRoomSideWays(0., movements);
            return 0;
        }
        
        if (currentRoomScale % 2 != 0 && neighbourRoomScale % 2 != 0) 
        {
            if (currentRoomScale == 1 || neighbourRoomScale == 1) movements = (highestScaleValue - 1) /2;
            else movements = (currentRoomScale + neighbourRoomScale);
            
            return MoveRoomSideWays(0, (int)movements);
        }
        
        if (currentRoomScale == 1 || neighbourRoomScale == 1) movements = (highestScaleValue - 1) / 2;
        else
        {
            if (currentRoomScale % 2 == 0) movements = currentRoomScale -1;
            else movements = neighbourRoomScale -1;
        }

        return MoveRoomSideWays(1, (int)movements);
    }

    private float MoveRoomSideWays (float initValue, int move)
    {
        float valueToReturn = initValue;
        int valueToAdd = Random.Range(0, move + 1);
        valueToReturn += valueToAdd *2;
        
        if (Random.Range(0, 2) % 2 == 0) valueToReturn *= -1;
        
        return valueToReturn;
    }

    private int GetRandomValue ()
    {
        int value = Random.Range(0, 2);

        return value == 0 ? -2 : 2;
    }

    private void GenerateDoors ()
    {
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            generatedRooms[i].CheckOverlapedWalls();
        }
    }

    private bool CheckIfThereAreOverlapedRooms ()
    {
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            if (generatedRooms[i].CheckIfIsInsideOfAnotherRoom())return true;
        }
        return false;
    }
}
