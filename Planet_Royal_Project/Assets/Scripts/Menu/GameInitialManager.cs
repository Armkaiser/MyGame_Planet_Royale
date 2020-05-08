using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameInitialManager : MonoBehaviourPunCallbacks
{
    public static GameInitialManager Instance;
    public GameObject startGameBtn;
    public GameObject InitGameMessage;

    public TextMeshProUGUI roomIDText;
    public TextMeshProUGUI descriptionText;

    bool connectToPhoton = false;
    bool isReadyToStart = false;

    private void Awake()
    {
        Instance = this;
        ConnectToPunServer();

        InitGameMessage.SetActive(true);
    }

    private void Update()
    {
        if (!connectToPhoton)
        {
            return;
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startGameBtn.SetActive(true);
            }
            else
            {
                startGameBtn.SetActive(false);
                isReadyToStart = false;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                isReadyToStart = true;
            }
            else
            {
                isReadyToStart = false;
            }

            descriptionText.text = "Waiting host starts the game. Players(" + PhotonNetwork.CurrentRoom.PlayerCount + "/4)";
        }
        else
        {
            isReadyToStart = false;
        }

    }

    ///<Summary>Connect to the pun Server</summary>
    private void ConnectToPunServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    ///<Summary>Connect to Lobby When successfully connect Pun Server</summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master!!");

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        InitGameMessage.SetActive(false);
    }

    ///<Summary>Call when join a Lobby</summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby!!");
        connectToPhoton = true;
        LoginManager.Instance.InitLoginUi();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnQuickJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LoginManager.Instance.OpenPopup("No room avaliable, You will create one.");
        LoginManager.Instance.IsInARoom(false);
        CreateRoom();
    }

    public void CreateRoom()
    {
        string roomID = Random.Range(10000, 99999).ToString();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomID, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join a room!");
        roomIDText.text = "Room ID : " + PhotonNetwork.CurrentRoom.Name;
        LoginManager.Instance.IsInARoom(true);
    }

    public void OnBtnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        LoginManager.Instance.IsInARoom(false);
        LoginManager.Instance.OpenPopup("You leave the room.");
    }

    public void OnBtnStartGame()
    {
        if (PhotonNetwork.IsMasterClient && isReadyToStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Gameplay");
        }
    }

}