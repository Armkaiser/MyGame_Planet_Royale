using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float baseDistance = 25f;
    [SerializeField] private float maxDistance = 35f;
    [SerializeField] private GameObject buttonSpectre = null;

    [SerializeField] private PlanetMovement[] otherPlayers;
    Transform target;
    Rigidbody currentPlayerRb;

    public const float playerVelocityDivide = 40;

    public static CameraController Instance;

    private int currentSpectreIndex = 0;
    private bool isPlayerDie = false;

    private void Awake()
    {
        Instance = this;
        buttonSpectre.SetActive(false);
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Follow(moveSpeed);
        }
    }

    private void Follow(float speed)
    {
        Vector3 playerPos = target.position;
        Vector3 desPos = new Vector3(playerPos.x, playerPos.y, -baseDistance + -currentPlayerRb.velocity.sqrMagnitude / playerVelocityDivide);

        desPos.z = Mathf.Clamp(desPos.z, -maxDistance, -baseDistance);

        transform.position = Vector3.Lerp(transform.position, desPos, speed * Time.deltaTime);
    }

    public void SetTarget(Transform value, bool setNewPlayer)
    {
        target = value;
        transform.position = new Vector3(value.position.x, value.position.y, -baseDistance);

        if (setNewPlayer)
        {
            PlanetMovement player = value.GetComponent<PlanetMovement>();

            if (player != null)
            {
                currentPlayerRb = player.GetComponent<Rigidbody>();
            }
        }
    }

    public void OnSpectreMode(int index)
    {
        currentSpectreIndex = index;
        isPlayerDie = true;
        buttonSpectre.SetActive(true);

        otherPlayers = FindObjectsOfType<PlanetMovement>();
        SetTarget(otherPlayers[index].gameObject.transform, true);
    }

    public void SwitchPlayerSpectre()
    {
        otherPlayers = FindObjectsOfType<PlanetMovement>();

        currentSpectreIndex++;
        if(currentSpectreIndex > otherPlayers.Length)
        {
            currentSpectreIndex = 0;
        }
        SetTarget(otherPlayers[currentSpectreIndex].gameObject.transform, true);
    }
}