using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public NodeJsAPI.PlayerData currentPlayerData;

    public static PlayerDataManager Instance;

     public bool isLoggedIn = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearData()
    {
        currentPlayerData.Username = "";
        currentPlayerData.WinCount = 0;
        currentPlayerData.KillCount = 0;
        currentPlayerData.DeadCount = 0;
        currentPlayerData.PlayCount = 0;
    }

    public void ReceiveData(NodeJsAPI.PlayerData data)
    {
        currentPlayerData.Username = data.Username;
        currentPlayerData.WinCount = data.WinCount;
        currentPlayerData.KillCount = data.KillCount;
        currentPlayerData.DeadCount = data.DeadCount;
        currentPlayerData.PlayCount = data.PlayCount;

        PhotonNetwork.LocalPlayer.NickName = currentPlayerData.Username;
    }

    public void AddDataWin()
    {
        currentPlayerData.WinCount += 1;
    }

    public void AddDataKill()
    {
        currentPlayerData.KillCount += 1;
    }

    public void AddDataDead()
    {
        currentPlayerData.DeadCount += 1;
    }

    public void AddDataPlay()
    {
        currentPlayerData.PlayCount += 1;
    }

    public void SubmitDataToCloud()
    {
        NodeJsAPI.Instance.UpdateData();
    }
}