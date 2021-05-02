using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


namespace MultiPlayer
{
    public class SocketConnection : MonoBehaviour
    {
        private WebSocket websocket;
        private List<string> messageQueue = new List<string>();
        private NetworkDataOption.ReplicateObjectList replicateListSend = new NetworkDataOption.ReplicateObjectList();

        public Room currentRoom;
        public  delegate void delegateHandle();
        public delegateHandle Onlogin;
        public delegateHandle OnCreateRoom;
        public delegateHandle OnJoinRoom;
        public delegateHandle OnLeaveRoom;
        public static SocketConnection instance;

        private void Awake()
        {
            instance = this;
        }       

        public void Connect()
        {
            string url = "ws://192.168.1.102:5500";
            InternalConnect(url);
        }

        private void InternalConnect(string url)
        {
            websocket = new WebSocket(url);
            websocket.Connect();

            websocket.OnMessage += OnMessage;
        }

        public bool IsConnected()
        {
            if (websocket == null)
                return false;

            return websocket.ReadyState == WebSocketState.Open;
        }

        // Update is called once per frame
        private void Update()
        {
            if(messageQueue.Count > 0)
            {
                NotifyCallback(messageQueue[0]);
                messageQueue.RemoveAt(0);
            }
        }

        private void NotifyCallback(string callbackData)
        {
            Debug.Log("OnMessage : " + callbackData);

            NetworkDataOption.EventServer recieveEvent = JsonUtility.FromJson<NetworkDataOption.EventServer>(callbackData);

            switch (recieveEvent.eventName)
            {
                case "Login":
                    {
                        NetworkDataOption.EventCallbackGeneral recieveEventGeneral = JsonUtility.FromJson<NetworkDataOption.EventCallbackGeneral>(callbackData);
                        Onlogin();
                        break;
                    }
                case "CreateRoom":
                    {
                        NetworkDataOption.EventCallbackGeneral recieveEventGeneral = JsonUtility.FromJson<NetworkDataOption.EventCallbackGeneral>(callbackData);
                        OnCreateRoom();
                        Internal_CreateRoom(recieveEventGeneral.data);                        
                        break;
                    }
                case "JoinRoom":
                    {
                        NetworkDataOption.EventCallbackGeneral recieveEventGeneral = JsonUtility.FromJson<NetworkDataOption.EventCallbackGeneral>(callbackData);
                        OnJoinRoom();
                        break;
                    }
                case "LeaveRoom":
                    {
                        NetworkDataOption.EventCallbackGeneral recieveEventGeneral = JsonUtility.FromJson<NetworkDataOption.EventCallbackGeneral>(callbackData);
                        OnLeaveRoom();
                        break;
                    }
                
            }
        }

        private IEnumerator IEUpdateReplicateObject()
        {
            float duration = 1.0f;

            WaitForSeconds waitForSec = new WaitForSeconds(duration);

            while (true)
            {
                string toJson = JsonUtility.ToJson(replicateListSend);

                SendReplicateData(toJson);

                yield return waitForSec;
            }
        }      

        private void SendReplicateData(string jsonStr)
        {
            NetworkDataOption.EventCallbackGeneral eventData = new NetworkDataOption.EventCallbackGeneral();
            eventData.eventName = "ReplicateData";
            eventData.data = jsonStr;

            websocket.Send(JsonUtility.ToJson(eventData));
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (websocket != null)
            {
                websocket.Close();
            }

        }

        public void Login(LoginData data)
        {
            NetworkDataOption.EventSendLoginData loginData = new NetworkDataOption.EventSendLoginData();
            loginData.eventName = "Login";
            loginData.userName = data.userName;
            loginData.password = data.password;

            websocket.Send(JsonUtility.ToJson(loginData));
        }

        public void Regis(RegisData data)
        {
            NetworkDataOption.EventSendRegisData regisData = new NetworkDataOption.EventSendRegisData();
            regisData.eventName = "Regis";
            regisData.userName = data.userName;
            regisData.name = data.name;
            regisData.password = data.password;

            Debug.Log(regisData);
            websocket.Send(JsonUtility.ToJson(regisData));
        }

        public void CreateRoom(Room.RoomOption roomOption)
        {
            NetworkDataOption.EventSendCreaateRoom evenData = new NetworkDataOption.EventSendCreaateRoom();
            evenData.eventName = "CreateRoom";
            evenData.roomOption = roomOption;
            
            websocket.Send(JsonUtility.ToJson(evenData));
        }

        public void JoinRoom(Room.RoomOption roomOption)
        {
            NetworkDataOption.EvenSendJoinRoom evenData = new NetworkDataOption.EvenSendJoinRoom();
            evenData.eventName = "JoinRoom";
            evenData.roomOption = roomOption;

            websocket.Send(JsonUtility.ToJson(evenData));
        }

        public void LeaveRoom(Room.RoomOption roomOption)
        {
            NetworkDataOption.EventSendLeaveRoom evenData = new NetworkDataOption.EventSendLeaveRoom();
            evenData.eventName = "LeaveRoom";
            evenData.roomOption = roomOption;

            websocket.Send(JsonUtility.ToJson(evenData));
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            messageQueue.Add(messageEventArgs.Data);
        }

        private void Internal_CreateRoom(string data)
        {
            Room.RoomOption roomOption = JsonUtility.FromJson<Room.RoomOption>(data);

            if(roomOption != null && currentRoom == null)
            {
                currentRoom = new Room();
                currentRoom.roomOption = roomOption;

                //StartCoroutine(IEUpdateReplicateObject());
            }
        }
       
    }
}
