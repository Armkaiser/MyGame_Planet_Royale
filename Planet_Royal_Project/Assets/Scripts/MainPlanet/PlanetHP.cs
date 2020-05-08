using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlanetHP : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Transform middleEarth = null;
    [SerializeField] List<Transform> planetShard = null;

    [Header("OnDamaged")]
    [SerializeField] Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    [SerializeField] float flashSpeed = 5f;

    private Image damageImage = null;
    private Image hpImage;

    [Range(0, 1)] private const float takeOffShardPerHpPercentage = 0.1f;
    private const float maxHp = 1000;

    [Range(0, 1)] private float currentShardPercentage;
    private int takeOffShardCount;

    private int hpWhenShardTakeOff;
    private float currentHp = maxHp;
    public bool isInCircle = true;

    bool takeDamaged;
    bool isDead = false;

    void Start()
    {
        takeOffShardCount = Mathf.RoundToInt(planetShard.Count * takeOffShardPerHpPercentage);
        currentShardPercentage = 1; //100%
        currentShardPercentage -= takeOffShardPerHpPercentage; //start at 90%
        hpWhenShardTakeOff = Mathf.RoundToInt(currentShardPercentage * maxHp);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (!isInCircle)
        {
            TakeDamage(100 * Time.deltaTime);
        }

        OnDamaged();
    }

    public void TakeDamage(float value, string killer = "")
    {
        takeDamaged = true;
        photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, value, killer);
    }

    void OnShardDamage(int shardToTakeOff)
    {
        if (planetShard.Count <= 0) return;

        for (int i = 0; i < shardToTakeOff; i++)
        {
            int randShard = Random.Range(0, planetShard.Count - 1);

            photonView.RPC("RPCExtractShard", RpcTarget.AllBuffered, randShard);
        }
    }

    void OnDamaged()
    {
        if (takeDamaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        takeDamaged = false;
    }

    public void SetPlayerHPUi(Image localHPImage, Image localDamageImage)
    {
        hpImage = localHPImage;
        damageImage = localDamageImage;

        hpImage.fillAmount = 1;
    }

    [PunRPC]
    void RPCExtractShard(int shardIndex)
    {
        if (planetShard[shardIndex] == null || planetShard.Count <= 0)
        {
            return;
        }

        Transform selectShard = planetShard[shardIndex];
        planetShard.RemoveAt(shardIndex);

        Rigidbody shard = selectShard.gameObject.AddComponent<Rigidbody>();
        Vector3 forceDir = selectShard.position - middleEarth.position;
        Vector3 torqueDir = Vector3.Cross(Vector3.forward, forceDir).normalized + forceDir;

        shard.useGravity = false;
        shard.AddForce(forceDir * 130);
        shard.AddTorque(torqueDir * 80);

        selectShard.SetParent(null);
        Destroy(selectShard.gameObject, 20);
    }

    [PunRPC]
    void RPCTakeDamage(float value, string killer)
    {
        currentHp -= value;

        if (currentHp <= 0 && !isDead)
        {
            if (killer == PlayerDataManager.Instance.currentPlayerData.Username)
            {
                PlayerDataManager.Instance.AddDataKill();
                isDead = true;
            }
        }

        if (!photonView.IsMine || MultiplayerManager.Instance.isEndGame) return;

        hpImage.fillAmount = (float) currentHp / (float) maxHp;

        if (currentHp <= 0)
        {
            OnShardDamage(planetShard.Count);
            GameplayManager.Instance.ClearAllEnemy();
            MultiplayerManager.Instance.RemoveMyObj(PhotonNetwork.LocalPlayer.NickName);
            CameraController.Instance.OnSpectreMode(0);
            PlayerDataManager.Instance.AddDataDead();
            PhotonNetwork.Destroy(gameObject);
        }
        else if (currentHp <= hpWhenShardTakeOff)
        {
            currentShardPercentage -= takeOffShardPerHpPercentage;
            hpWhenShardTakeOff = Mathf.RoundToInt(currentShardPercentage * maxHp);

            OnShardDamage(takeOffShardCount);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.Serialize(ref currentHp);
    }
}