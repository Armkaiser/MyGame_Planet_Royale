using System.Collections;
using System.Collections.Generic;
using NorapatUtility;
using Photon.Pun;
using UnityEngine;

public class EnemyShipController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float hp = 100f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 2f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float fireDistance = 40f;
    [SerializeField] Transform muzzle = null;
    [SerializeField] List<Renderer> renderers = null;

    private Transform target;
    private Rigidbody rb;
    private Collider col;
    private Vector3 offsetPos;
    private float fireRateCount;
    private bool isDead;

    Vector3 positionValue;
    Quaternion rotationValue;

    float deadTimer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        isDead = false;
        fireRateCount = fireRate;

        deadTimer = -1;
    }

    private void Update()
    {
        if (isDead)
        {
            deadTimer += Time.deltaTime;
            foreach(Renderer r in renderers)
            {
                foreach(Material m in r.materials)
                {
                    m.SetFloat("Vector1_A43C27DA", deadTimer);
                }
            }

            return;
        }

        if (!photonView.IsMine)
        {
            rb.angularVelocity = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, positionValue, 7 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 7 * Time.deltaTime);
            return;
        }

        if (target == null) return;

        Vector3 dirToTarget = target.position - transform.position;
        float dirMagnitude = dirToTarget.magnitude;

        TargetRotate(dirToTarget + offsetPos / 3);
        
        if (dirMagnitude < 7)
        {
            rb.drag = 10;
        }
        else
        {
            rb.drag = 0.5f;
        }
        
        fireRateCount -= Time.deltaTime;

        if (fireRateCount <= 0 && dirMagnitude < fireDistance)
        {
            ShootBullet();
        }

        Move();
    }

    void TargetRotate(Vector3 dir)
    {
        TransUtility.RotateTowardToDir(transform, -dir, Vector3.forward, rotateSpeed * Time.deltaTime, Quaternion.Euler(-90, 0, 0));
    }

    void ShootBullet()
    {
        GameObject newObj = PhotonNetwork.Instantiate("Bullet_Prototype_Enemy", muzzle.position, muzzle.rotation);
        Physics.IgnoreCollision(GetComponent<Collider>(), newObj.GetComponent<Collider>());

        float randX = Random.Range(-15, 15);
        float randY = Random.Range(-15, 15);

        if(randX < 10 && randX >= 0)
        {
            randX = 10;
        }
        else if(randX > -10 && randX <= 0)
        {
            randX = -10;
        }

        if (randY < 10 && randY >= 0)
        {
            randY = 10;
        }
        else if (randY > -10 && randY <= 0)
        {
            randY = -10;
        }

        offsetPos = new Vector3(randX, randY, 0);

        fireRateCount = fireRate;
    }

    void Move()
    {
        Vector3 dirToTargetOffSet = (target.position + offsetPos) - transform.position;

        rb.AddForce(dirToTargetOffSet.normalized * moveSpeed * Time.deltaTime);
        rb.angularVelocity = Vector3.zero;

        positionValue = transform.position;
        rotationValue = transform.rotation;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (isDead) return;

        stream.Serialize(ref positionValue);
        stream.Serialize(ref rotationValue);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void TakeDamage(float value)
    {
        photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    void RPCTakeDamage(float value)
    {
        hp -= value;
        
        if (hp <= 0)
        {
            isDead = true;
            col.enabled = false;

            if (photonView.IsMine)
            {
                Invoke("ClearObject", 3);
            }
        }
    }

    void ClearObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}