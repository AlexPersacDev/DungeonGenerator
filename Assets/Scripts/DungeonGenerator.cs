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

            if (SetRandomRoomPos() < 0)
            {
                float zPos = 0;
                if(generatedRooms[i].transform.localScale.z!= 1) 
                    zPos = (generatedRooms[i].transform.localPosition.z - generatedRooms[i].transform.localScale.z)/SetRandomRoomPos();
                
                generatedRooms[i].transform.localPosition += new Vector3(
                (neighborCurrentRoom.transform.localScale.x + generatedRooms[i].transform.localScale.x)/2 * SetRandomRoomPos(), 
                        generatedRooms[i].transform.localPosition.y, zPos);
            }

            else
            {
                float xPos = 0;
                if(generatedRooms[i].transform.localScale.x!= 1) 
                    xPos = (generatedRooms[i].transform.localPosition.x - generatedRooms[i].transform.localScale.x)/SetRandomRoomPos();
                
                generatedRooms[i].transform.localPosition += new Vector3(xPos, generatedRooms[i].transform.localPosition.y, 
                    (neighborCurrentRoom.transform.localScale.z + generatedRooms[i].transform.localScale.z)/2 * SetRandomRoomPos());
            }
                
            
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
                
                if (SetRandomRoomPos() < 0)
                {
                    float zPos = 0;
                    if(generatedRooms[i].transform.localScale.z != 1) 
                        zPos = (generatedRooms[i].transform.localPosition.z - generatedRooms[i].transform.localScale.z)/SetRandomRoomPos();
                
                    generatedRooms[i].transform.localPosition += new Vector3(
                        (neighborCurrentRoom.transform.localScale.x + generatedRooms[i].transform.localScale.x)/2 * SetRandomRoomPos(), 
                        generatedRooms[i].transform.localPosition.y, zPos);
                }

                else
                {
                    float xPos = 0;
                    if(generatedRooms[i].transform.localScale.x != 1) 
                        xPos = (generatedRooms[i].transform.localPosition.x - generatedRooms[i].transform.localScale.x)/SetRandomRoomPos();
                
                    generatedRooms[i].transform.localPosition += new Vector3(xPos, generatedRooms[i].transform.localPosition.y, 
                        (neighborCurrentRoom.transform.localScale.z + generatedRooms[i].transform.localScale.z)/2 * SetRandomRoomPos());
                }
            }
            generatedRooms[i].GenerateRoom(); 
        }
    }

    private int SetRandomRoomPos ()
    {
        int value = Random.Range(0, 2);
        if (value == 0) return -2;
        
        return 2;
    }
}
