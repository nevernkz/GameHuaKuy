using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MultiPlayer;

public class UiManager : MonoBehaviour
{ 
    public GameObject panel_Login, panel_Regis, panel_StatausRoom, panel_CreateRoom, panel_JoinRoom, panel_Lobby, panel_StatusRoomText;
    public Button logInButton, regisButton, regisOkButton, createRoomButton, joinRoomButton, createOkButton, joinOkButton;
    public InputField inputRoomName, inputRoomPassword,inputLoginUserName,inputLoginPassword,inputJoinRoomName,inputJoinRoomPassword, inputUserName, inputName, inputPassword;
    public Text lobby_roomName, lobby_PlayerName, lobby_Status, statausRoomName, statusPlayerNum;

    private int playerNum = 0;
    private int clicked = 0;
    void Start()
    {
        Menu();
        regisButton.onClick.AddListener(Regis);

        SocketConnection.instance.Onlogin += Internal_Login;
        SocketConnection.instance.OnCreateRoom += Internal_CreateRoom;
        SocketConnection.instance.OnJoinRoom += Internal_JoinRoom;
        SocketConnection.instance.OnLeaveRoom += Internal_LeaveRoom;
    }   

    public void Menu()
    {
        panel_Login.SetActive(true);
        panel_Regis.SetActive(false);
        panel_StatausRoom.SetActive(false);
        panel_CreateRoom.SetActive(false);
        panel_JoinRoom.SetActive(false);
        panel_Lobby.SetActive(false);
    }

    private void Login()
    {
        panel_StatausRoom.SetActive(true);
        panel_Login.SetActive(false);        
        
    }

    private void Internal_Login()
    {
        Login();
        
    }

    private void StatusRoomText(int playerNum)
    {
        
        statausRoomName.text = inputRoomName.text;
        statusPlayerNum.text = playerNum + "/3 Player";
        
    }

    private void Internal_CreateRoom()
    {
        Lobby();
        playerNum += 1;
        StatusRoomText(playerNum);
        panel_StatusRoomText.SetActive(true);

        lobby_roomName.text = "#" + inputRoomName.text;
        lobby_PlayerName.text = inputLoginUserName.text;
        lobby_Status.text = "Not Ready";
    }

    private void Internal_JoinRoom()
    {
        Lobby();
        playerNum += 1;
        StatusRoomText(playerNum);

        lobby_roomName.text = "#" + inputJoinRoomName.text;
        lobby_PlayerName.text = inputLoginUserName.text;
        lobby_Status.text = "Not Ready";
    }

    private void Internal_LeaveRoom()
    {
        playerNum -= 1;
        StatusRoomText(playerNum);

        lobby_roomName.text = "";
        lobby_PlayerName.text = "";
        lobby_Status.text = "";

        if (playerNum <= 0)
        {
            panel_StatusRoomText.SetActive(false);
        }
    }

    private void Regis()
    {
        panel_Login.SetActive(false);
        panel_Regis.SetActive(true);
    }

    private void CreateRoom()
    {
        panel_CreateRoom.SetActive(true);             
    }

    private void JoinRoom()
    {
        panel_JoinRoom.SetActive(true);        
    }

    private void LeaveRoom()
    {
        panel_StatausRoom.SetActive(true);
        panel_Lobby.SetActive(false);
    }

    private void Lobby()
    {
        panel_Lobby.SetActive(true);
        panel_Login.SetActive(false);
        panel_StatausRoom.SetActive(false);
        panel_CreateRoom.SetActive(false);
        panel_JoinRoom.SetActive(false);
    }

    public void BtnClick(string btnText)
    {
        if (SocketConnection.instance.IsConnected() == false)
        {
            if (btnText == "Login")
            {                
                SocketConnection.instance.Connect();

                LoginData loginData = new LoginData();
                loginData.userName = inputLoginUserName.text;
                loginData.password = inputLoginPassword.text;

                SocketConnection.instance.Login(loginData);
            }            

            else if (btnText == "OkRegis")
            {
                Menu();
                SocketConnection.instance.Connect();

                RegisData regisData = new RegisData();
                regisData.userName = inputUserName.text;
                regisData.name = inputName.text;
                regisData.password = inputPassword.text;
                
                SocketConnection.instance.Regis(regisData);
            }

        }        

        else
        {
            if (SocketConnection.instance.currentRoom == null)
            {               
                if (btnText =="CreateRoom")
                {
                    CreateRoom();
                    
                }
                else if (btnText == "JoinRoom")
                {                    
                    JoinRoom();
                }
            }
            else
            {
                if(btnText == "JoinRoom")
                {                    
                    JoinRoom();                    
                }                
            }
            
        }
    }
    public void BtnOkClick(string btnText)
    {
        

        if (btnText == "OkCreateRoom")
        {
            Lobby();            
            Room.RoomOption roomOption = new Room.RoomOption();
            roomOption.roomName = inputRoomName.text;
            roomOption.roomPassword = inputRoomPassword.text;
            roomOption.playerNum += 1;

            SocketConnection.instance.CreateRoom(roomOption);
        }

        else if (btnText == "OkJoinRoom")
        {
            Lobby();
            Room.RoomOption roomOption = new Room.RoomOption();
            roomOption.roomName = inputJoinRoomName.text;
            roomOption.roomPassword = inputJoinRoomPassword.text;
            roomOption.playerNum += 1;

            SocketConnection.instance.JoinRoom(roomOption);
        }
        else if (btnText == "OkLeaveRoom")
        {
            LeaveRoom();
            Room.RoomOption roomOption = new Room.RoomOption();
            roomOption.roomName = statausRoomName.text;
            roomOption.playerNum -= 1;

            SocketConnection.instance.LeaveRoom(roomOption);
        }
        else if(btnText == "Ready")
        {
            clicked += 1;

            if (clicked % 2 == 1)
            {
                lobby_Status.text = "Ready";
                Invoke("LoadToGamePlay", 3f);
            }
              

            else if (clicked % 2 == 0)
                lobby_Status.text = "Not Readey";

        }
    }

    private void LoadToGamePlay()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
