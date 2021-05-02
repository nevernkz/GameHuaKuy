using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlayer
{
    public class NetworkDataOption
    {
        [Serializable]
        public class ReplicateObject
        {
            public string objectID;
            public string ownerID;
            public string prefName;
            public Vector3 position;
        }

        [Serializable]
        public class ReplicateObjectList
        {
            public List<ReplicateObject> replicateObject = new List<ReplicateObject>();
        }

        [Serializable]
        public class EventCallbackGeneral
        {
            public string eventName;
            public string data;
        }
        public class EventServer : EventCallbackGeneral
        {
            public Room.RoomOption roomOption;
        }

        [Serializable]
        public class EventSendCreaateRoom : EventCallbackGeneral
        {
            public Room.RoomOption roomOption;
        }

        public class EvenSendJoinRoom : EventCallbackGeneral
        {
            public Room.RoomOption roomOption;
        }

        public class EventSendLeaveRoom : EventCallbackGeneral
        {
            public Room.RoomOption roomOption;
        }
        public class EventSendRegisData : RegisData
        {
            public RegisData regisData;
        }

        public class EventSendLoginData : LoginData
        {
            public LoginData loginData;
        }

        public class EventSendPlayerStatus: PlayerStatus
        {
            public PlayerStatus playerStatus;
        }
    }

    public class RegisData
    {
        public string eventName;
        public string userName;
        public string name;
        public string password;
    }

    public class LoginData
    {
        public string eventName;
        public string userName;
        public string password;
    }
    
    public class PlayerStatus
    {
        public string status;
    }

    public class Room
    {
        [Serializable]
        public class RoomOption
        {
            public string status;
            public string userName;
            public string roomName;
            public string roomPassword;
            public int playerNum;
        }

        public RoomOption roomOption;
    }
}

