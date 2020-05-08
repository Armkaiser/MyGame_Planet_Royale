using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyBullet : MonoBehaviourPun
{
    [SerializeField] int damage = 20;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float moveSpeed = 15;
    Rigidbody rb;

    float lifeTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        BulletMovemet();
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
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView view = other.gameObject.GetComponent<PhotonView>();

            if ( view != null)
            {
                if(view.IsMine)
                {
                    if(!photonView.IsMine)
                    {
                        photonView.TransferOwnership(view.Owner);
                    }
                    
                    PlanetHP PlanetHP = view.GetComponent<PlanetHP>();
                    
                    if (PlanetHP != null)
                    {
                        PlanetHP.TakeDamage(damage);
                    }

                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
        else if(!other.gameObject.CompareTag("Bullet"))
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
           
    }

}