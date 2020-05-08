using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlanetWeapon : MonoBehaviourPun
{
    [SerializeField] Collider playerCol = null;
    [SerializeField] GameObject bulletPref = null;
    [SerializeField] Transform muzzle = null;
    [SerializeField][Range(0, 2)] float fireCooldown = 0.4f;

    public const string PLAYER_BULLET_NAME = "Bullet_Prototype";

    float fireRateTimer;
    string bulletPath;

    // Start is called before the first frame update
    void Start()
    {
        bulletPath = GameplayManager.PLANET_FOLDER_NAME + PLAYER_BULLET_NAME;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButton(0) && fireRateTimer <= 0)
        {
            GameObject newObj = PhotonNetwork.Instantiate(bulletPath, muzzle.position, muzzle.rotation);
            Physics.IgnoreCollision(playerCol, newObj.GetComponent<Collider>());
            fireRateTimer = fireCooldown;
        }

        fireRateTimer -= Time.deltaTime;
    }
}