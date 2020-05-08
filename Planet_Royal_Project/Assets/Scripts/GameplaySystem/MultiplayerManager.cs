using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public Transform LocalCanvas = null;
    [SerializeField] private List<string> allplayerAlive = null;

    public GameObject panelGameEnd;
    public Text panelGameEndText;
    public static MultiplayerManager Instance;

    public bool isStartGame = false;
    public bool isEndGame = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitToStartGame());
        panelGameEnd.SetActive(false);
        photonView.RPC("RPCAddNewObj", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        PlayerDataManager.Instance.AddDataPlay();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (isStartGame)
        {
            if (allplayerAlive.Count <= 1)
            {
                photonView.RPC("RPCEndGame", RpcTarget.AllBuffered, allplayerAlive[0]);
                isStartGame = false;
            }
        }
    }

    IEnumerator WaitToStartGame()
    {
        yield return new WaitForSecondsRealtime(10);
        isStartGame = true;
    }

    public void RemoveMyObj(string playerId)
    {
        photonView.RPC("RPCRemove", RpcTarget.AllBuffered, playerId);
    }

    public void OnBtnExit()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    [PunRPC]
    void RPCAddNewObj(string playerId)
    {
        allplayerAlive.Add(playerId);
    }

    [PunRPC]
    void RPCRemove(string playerId)
    {
        allplayerAlive.Remove(playerId);
    }

    [PunRPC]
    void RPCEndGame(string winner)
    {
        panelGameEnd.SetActive(true);
        panelGameEndText.text = "The winner is.....\n" + winner;
        if (winner == PlayerDataManager.Instance.currentPlayerData.Username)
        {
            PlayerDataManager.Instance.AddDataWin();
        }
        isStartGame = false;
        isEndGame = true;

        Invoke("OnBtnExit", 10);
    }
}