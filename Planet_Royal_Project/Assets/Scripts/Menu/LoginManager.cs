using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [System.Serializable]
    public struct PanelData
    {
        public string key;
        public GameObject panelObj;
    }

    [System.Serializable]
    public struct PopUpPanelData
    {
        public GameObject panelPopup;
        public TextMeshProUGUI textMessage;
    }

    public List<PanelData> panelList = new List<PanelData>();

    public GameObject InLobbyUi;
    public GameObject InARoomUi;

    public PopUpPanelData panelPopupData;

    public Dictionary<string, GameObject> panelDic = new Dictionary<string, GameObject>();

    public static LoginManager Instance;

    private void Awake()
    {
        Instance = this;
        InitPanelData();
        CloseAllPanel();
    }

    public void InitLoginUi()
    {
        IsInARoom(false);

        if (PlayerDataManager.Instance.isLoggedIn)
        {
            NodeJsAPI.Instance.ShowData(PlayerDataManager.Instance.currentPlayerData);
            PlayerDataManager.Instance.SubmitDataToCloud();
            OpenPanel("Lobby");
        }
        else
        {
            OpenPanel("Selection");
        }
    }

    void InitPanelData()
    {
        foreach (var panelData in panelList)
        {
            panelDic.Add(panelData.key, panelData.panelObj);
        }
    }

    public void OpenPanel(string key)
    {
        CloseAllPanel();

        panelDic[key].SetActive(true);
    }

    public void OpenPopup(string message)
    {
        panelPopupData.textMessage.text = message;
        panelPopupData.panelPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        panelPopupData.panelPopup.SetActive(false);
    }

    public void CloseAllPanel()
    {
        foreach (var panelData in panelDic)
        {
            panelData.Value.SetActive(false);
        }
    }

    public void LogOut()
    {
        PlayerDataManager.Instance.ClearData();
        PlayerDataManager.Instance.isLoggedIn = false;
        OpenPopup("Logout success!");
        OpenPanel("Selection");
    }

    public void IsInARoom(bool value)
    {
        InARoomUi.SetActive(value);
        InLobbyUi.SetActive(!value);
    }
}