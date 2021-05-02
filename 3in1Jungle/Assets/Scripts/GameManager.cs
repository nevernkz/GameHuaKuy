using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiPlayer;

public class GameManager : MonoBehaviour
{
    public string roomName;
    public void OnGUI()
    {
        if(SocketConnection.instance.IsConnected() == false)
        {
            if (GUILayout.Button("Connect"))
            {
                SocketConnection.instance.Connect();
            }
        }
        else
        {
            if(SocketConnection.instance.currentRoom == null)
            {
                roomName = GUILayout.TextField(roomName);

                if (GUILayout.Button("CreateRoom"))
                {
                    Room.RoomOption roomOption = new Room.RoomOption();
                    roomOption.roomName = roomName;

                    SocketConnection.instance.CreateRoom(roomOption);
                }
            }
        }
    }
}
