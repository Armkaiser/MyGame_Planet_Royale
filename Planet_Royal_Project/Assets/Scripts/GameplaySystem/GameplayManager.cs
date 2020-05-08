using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviourPunCallbacks
{
    [Header("Player Ui")]
    [SerializeField] Image energyBar = null;
    [SerializeField] Image hpBar = null;
    [SerializeField] Image damagedImage = null;
    [SerializeField] Transform[] spawnPos = null;

    PlanetMovement currentPlayer;
    public const string PLANET_FOLDER_NAME = "PlayerPlanet/";
    public const string PLAYER_PLANER_NAME = "Player_Planet_Demo";

    float spawnTimer;

    [SerializeField] private float timeSpawn = 12f; // ระยะเวลาในการ spawn
    [SerializeField] private float distFrPlayer = 30f; // ระยะห่างจาก player
    [SerializeField] private float maxEnemy = 5f; // ระยะห่างจาก player

    List<GameObject> enemyList;

    public static GameplayManager Instance;

    private void Awake()
    {
        Instance = this;
        spawnTimer = timeSpawn;
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(10, 10);

        PhotonNetwork.AutomaticallySyncScene = false;

        enemyList = new List<GameObject>();

        GameObject newObj = PhotonNetwork.Instantiate(PLANET_FOLDER_NAME + PLAYER_PLANER_NAME,
            spawnPos[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity, 0);

        currentPlayer = newObj.GetComponent<PlanetMovement>();
        currentPlayer.SetPlayerMovementUi(energyBar);
        currentPlayer.GetComponent<PlanetHP>().SetPlayerHPUi(hpBar, damagedImage);
    }

    void Update()
    {
        if (currentPlayer == null)
        {
            if (enemyList.Count > 0)
            {
                ClearAllEnemy();
            }

            return;
        }

        if (enemyList.Count < 6 && MultiplayerManager.Instance.isStartGame)
        {
            spawnTimer -= Time.deltaTime;
        }

        if (spawnTimer <= 0)
        {
            Spawn();
            Spawn();
            spawnTimer = timeSpawn;
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
            }
        }
    }

    public void Spawn()
    {
        float randAngle = Random.Range(0f, 360f); // random มุมที่ spawn
        float X = Mathf.Cos(randAngle);
        float Y = Mathf.Sin(randAngle);

        Vector3 circlePos = new Vector3(X, Y); // จุดเกิดห่างจาก player 1 หน่วย
        circlePos *= distFrPlayer; // ผลักให้ไกลออกไป
        circlePos += currentPlayer.transform.position; // ให้เกิดตามตำแหน่ง player ด้วย

        GameObject newObj = PhotonNetwork.Instantiate("Enemy_Ship", circlePos, Quaternion.identity);
        enemyList.Add(newObj);

        newObj.GetComponent<EnemyShipController>().SetTarget(currentPlayer.transform);

        spawnTimer = 5f;
    }

    public void ClearAllEnemy()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            EnemyShipController enemy = enemyList[i].GetComponent<EnemyShipController>();
            enemy.TakeDamage(1000);
            enemyList.RemoveAt(i);
        }
    }
}