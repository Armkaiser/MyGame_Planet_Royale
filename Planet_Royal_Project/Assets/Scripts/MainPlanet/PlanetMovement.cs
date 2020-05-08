using System.Collections;
using System.Collections.Generic;
using NorapatUtility;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlanetMovement : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float speed = 500;
    [SerializeField] float timeBeforeRegen = 0.5f;
    [SerializeField][Range(0f, 1f)] float engineDrainRate = 0.1f;
    [SerializeField][Range(0f, 1f)] float engineRegenRate = 0.2f;

    // All ui set by Gameplay Manager
    [HideInInspector] public Image energyImage = null;

    float timeBeforeRegenTimer;
    bool isLockEngine;

    Color energyColor;
    Rigidbody rb;

    Vector3 positionValue;
    Quaternion rotationValue;

    private void Start()
    {
        gameObject.name = PhotonNetwork.LocalPlayer.NickName;

        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            energyColor = energyImage.color;
            isLockEngine = false;
            CameraController.Instance.SetTarget(transform, true);
        }
        else
        {
            PointerManager.Instance.RequestNewPlayerPointer(transform, GetComponent<PhotonView>().Owner.NickName);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            TargetMouse();

            positionValue = transform.position;
            rotationValue = transform.rotation;
        }
        else
        {
            Vector3 pos = positionValue;
            Quaternion angle = rotationValue;

            rb.angularVelocity = Vector3.zero;

            transform.position = Vector3.Lerp(transform.position, pos, 7 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, angle, 7 * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetMouseButton(1) && energyImage.fillAmount > 0 && !isLockEngine)
        {
            JetEngineActivate();
        }
        else if (timeBeforeRegenTimer <= 0)
        {
            energyImage.fillAmount += engineRegenRate * Time.deltaTime;

            if (isLockEngine && energyImage.fillAmount >= 1)
            {
                isLockEngine = false;
                energyImage.color = energyColor;
            }
        }

        timeBeforeRegenTimer -= Time.deltaTime;
    }

    void JetEngineActivate()
    {
        energyImage.fillAmount -= engineDrainRate * Time.deltaTime;

        AddForceRigidBody();

        timeBeforeRegenTimer = timeBeforeRegen;

        if (energyImage.fillAmount <= 0)
        {
            isLockEngine = true;
            timeBeforeRegenTimer = 0;

            energyImage.color = Color.red;
        }
    }

    private void AddForceRigidBody()
    {
        Vector3 direction = transform.up;
        direction.z = 0;
        rb.AddForce(direction * speed);
    }

    private void TargetMouse() // Aim
    {
        rb.angularVelocity = Vector3.zero;
        Vector3 dir = TransUtility.GetMouseDirOnScreen(transform.position);
        TransUtility.RotateSmoothToDir(transform, dir * -1, Vector3.forward, 5 * Time.deltaTime, Quaternion.Euler(-90, 0, 0));
    }

    public void SetPlayerMovementUi(Image localEnergyImage)
    {
        energyImage = localEnergyImage;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.Serialize(ref positionValue);
        stream.Serialize(ref rotationValue);
    }
}