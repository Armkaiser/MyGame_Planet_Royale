using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BulletController : MonoBehaviourPun
{
    [SerializeField] int damage = 30;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] bool playerBullet = false;
    Rigidbody rb;

    float lifeTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        BulletMovemet();

        if(!photonView.IsMine)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void BulletMovemet()
    {
        rb.velocity = transform.up * moveSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine && lifeTimer < 0.5f) return;

        PhotonView hitobj = other.gameObject.GetComponent<PhotonView>();

        if (hitobj != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!hitobj.IsMine)
                {
                    PlanetHP PlanetHP = hitobj.GetComponent<PlanetHP>();

                    if (PlanetHP != null)
                    {
                        if(playerBullet)
                        {
                            PlanetHP.TakeDamage(damage, PhotonNetwork.LocalPlayer.NickName);
                        }
                        else
                        {
                            PlanetHP.TakeDamage(damage);
                        }
                        
                    }
                }
            }
            else if(other.gameObject.CompareTag("Enemy"))
            {
                EnemyShipController enemy = hitobj.GetComponent<EnemyShipController>();

                if(enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}