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
    private GameObject neighborCurrentRoom;
    
    // Properties----

    // Unity Events----
    private void Start ()
    {
        generatedRooms.Add(Instantiate(room, Vector3.zero, Quaternion.identity));
        generatedRooms[0].InitRoom(new Vector2Int(0, 1)); 
        generatedRooms[0].GenerateRoom(); 
        roomsCant--;
        GenerateRooms();
    }

    // Public Methods----

    // Private Methods----
    private async void GenerateRooms ()
    {
        for (int i = 1; i < roomsCant; i++)
        {
            neighborCurrentRoom = generatedRooms[Random.Range(0, generatedRooms.Count)].gameObject;
            
            generatedRooms.Add(Instantiate(room, 
                neighborCurrentRoom.transform.position, 
                    Quaternion.identity));

            generatedRooms[i].InitRoom(Vector2Int.zero);
            
            PutRoomInNeighbourBounds(generatedRooms[i], neighborCurrentRoom);
            
            generatedRooms[i].GenerateRoom();
            await Task.Delay(1);
            
            if (!generatedRooms[i].CheckIfIsInsideOfanotherRoom()) continue;
            
            while (generatedRooms[i].CheckIfIsInsideOfanotherRoom())
            {
                await Task.Delay(1);
                int indx = Random.Range(0, generatedRooms.Count);
                neighborCurrentRoom = generatedRooms[indx].gameObject;
                print(indx);
                generatedRooms[i].transform.position = neighborCurrentRoom.transform.position;
                
                generatedRooms[i].RescaleRoom();
                
                PutRoomInNeighbourBounds(generatedRooms[i], neighborCurrentRoom);
            }
            generatedRooms[i].GenerateRoom(); 
        }
    }

    private void PutRoomInNeighbourBounds (RoomGenerator currentRoom, GameObject neighbourRoom)
    {
        float zValue = neighbourRoom.transform.localScale.z + currentRoom.transform.localScale.z;
        float xValue = neighbourRoom.transform.localScale.x + currentRoom.transform.localScale.x;;
        
        if (GetRandomValue() < 0)
        {
            currentRoom.transform.position +=
                new Vector3(xValue / GetRandomValue(),
                    transform.position.y, zValue);
        }

        else
        {
            currentRoom.transform.position +=
                new Vector3(xValue,
                    transform.position.y, zValue / GetRandomValue());
        }
    }

    private int GetRandomValue ()
    {
        int value = Random.Range(0, 2);

        return value == 0 ? -2 : 2;
    }
}
