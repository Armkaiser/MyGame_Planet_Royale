using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NodeJsAPI : MonoBehaviour
{
    [System.Serializable]
    public struct CallbackMessage
    {
        public string status;
        public string detail;
        public PlayerData[] data;
    }

    [System.Serializable]
    public struct RegisterData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public struct PlayerData //ชื่อตัวแปรต้องตรงกับ Data base
    {
        public string Username;
        public int WinCount;
        public int KillCount;
        public int DeadCount;
        public int PlayCount;
    }

    string currentUserName;

    public delegate void Callback();

    [SerializeField] TextMeshProUGUI headerText = null;
    [SerializeField] TextMeshProUGUI StatsWin = null;
    [SerializeField] TextMeshProUGUI StatsKill = null;
    [SerializeField] TextMeshProUGUI StatsDead = null;
    [SerializeField] TextMeshProUGUI StatsPlay = null;

    [Header("Input")]
    public TMP_InputField usernameRegisterInput;
    public TMP_InputField passwordRegisterInput;
    public TMP_InputField confirmPasswordInput;

    public TMP_InputField usernameLoginInput;
    public TMP_InputField passwordLoginInput;

    string defaultUrl = "https://sqllearn-276509.df.r.appspot.com/";

    public static NodeJsAPI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Register()
    {
        if (passwordRegisterInput.text.Length > 11)
        {
            LoginManager.Instance.OpenPopup("Password is tool long. (4-11) characters.");
        }
        else if (passwordRegisterInput.text.Length < 4)
        {
            LoginManager.Instance.OpenPopup("Password is tool short. (4-11) characters.");
        }
        else if (passwordRegisterInput.text != confirmPasswordInput.text)
        {
            LoginManager.Instance.OpenPopup("Password and Confirm-Password doesn't match.");
        }
        else if (usernameRegisterInput.text.Length > 10)
        {
            LoginManager.Instance.OpenPopup("username is tool long. (4-10) characters.");
        }
        else if (usernameRegisterInput.text.Length < 4)
        {
            LoginManager.Instance.OpenPopup("username is tool short. (4-10) characters.");
        }
        else
        {
            string url = defaultUrl + "register";

            var registerData = new RegisterData();
            registerData.username = usernameRegisterInput.text;
            registerData.password = passwordRegisterInput.text;

            string jsonConvert = JsonUtility.ToJson(registerData); //Convert to json

            Debug.Log(jsonConvert);

            StartCoroutine(WebRequest(url, "POST", jsonConvert, "Register complete!", null));
        }
    }

    public void Login()
    {
        string url = defaultUrl + "login";

        var loginData = new RegisterData();
        loginData.username = usernameLoginInput.text;
        loginData.password = passwordLoginInput.text;

        string jsonConvert = JsonUtility.ToJson(loginData); //Convert to json
        StartCoroutine(WebRequest(url, "POST", jsonConvert, "Login complete!", GetPlayerData));
    }

    public void GetPlayerData() //call data
    {
        string url = defaultUrl + "PlayerData";

        var loginData = new RegisterData();
        loginData.username = currentUserName;

        string jsonConvert = JsonUtility.ToJson(loginData); //Convert to json
        StartCoroutine(WebRequest(url, "POST", jsonConvert, "Login and get data success.", null));
    }

    public void UpdateData()
    {
        string url = defaultUrl + "UpdatePlayer";

        var playerData = new PlayerData();
        playerData = PlayerDataManager.Instance.currentPlayerData;

        string jsonConvert = JsonUtility.ToJson(playerData); //Convert to json
        StartCoroutine(WebRequest(url, "POST", jsonConvert, "Update data successful.", null));
        ShowData(PlayerDataManager.Instance.currentPlayerData);
    }

    IEnumerator WebRequest(string url, string method, string jsonStr, string successMessage, Callback callbackSuccess)
    {
        Debug.Log(jsonStr);

        var webRequest = new UnityWebRequest(url); //สร้าง Request
        webRequest.method = method;

        if (jsonStr != "") //แปลง Class เป็น json และส่งเป็น Text ออกไป
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw); //Upload
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        webRequest.downloadHandler = new DownloadHandlerBuffer(); //Buffer รอการโหลดและโหลด res จาก node

        yield return webRequest.SendWebRequest(); //ส่งข้อมูล Upload ออกไป

        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            LoginManager.Instance.OpenPopup("Network error, " + webRequest.error);
            Debug.Log(webRequest.error);
        }
        else
        {
            //รับ object จาก res.send
            CallbackMessage receiveCallBack = JsonUtility.FromJson<CallbackMessage>(webRequest.downloadHandler.text);

            if (receiveCallBack.status == "success")
            {
                LoginManager.Instance.OpenPopup(successMessage);

                if (successMessage == "Register complete!")
                {
                    LoginManager.Instance.OpenPanel("Login");
                    usernameRegisterInput.text = "";
                    passwordRegisterInput.text = "";
                    confirmPasswordInput.text = "";
                    usernameLoginInput.text = "";
                    passwordLoginInput.text = "";
                }
                else if (successMessage == "Login complete!")
                {
                    currentUserName = usernameLoginInput.text;
                    usernameLoginInput.text = "";
                    passwordLoginInput.text = "";
                }
                else if (successMessage == "Login and get data success.") //LoadData
                {
                    PlayerDataManager.Instance.ReceiveData(receiveCallBack.data[0]);
                    ShowData(PlayerDataManager.Instance.currentPlayerData);
                    LoginManager.Instance.OpenPanel("Lobby");
                    PlayerDataManager.Instance.isLoggedIn = true;
                }

                if (callbackSuccess != null)
                {
                    callbackSuccess();
                }
            }
            else
            {
                LoginManager.Instance.OpenPopup(receiveCallBack.detail);
            }

            Debug.Log(webRequest.downloadHandler.text);
        }
    }

    public void ShowData(PlayerData playerData)
    {
        headerText.text = "Welcome, " + playerData.Username;
        StatsWin.text = playerData.WinCount.ToString();
        StatsKill.text = playerData.KillCount.ToString();
        StatsDead.text = playerData.DeadCount.ToString();
        StatsPlay.text = playerData.PlayCount.ToString();
    }
}