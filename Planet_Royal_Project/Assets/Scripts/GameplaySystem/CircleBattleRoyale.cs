using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CircleBattleRoyale : MonoBehaviourPun
{
    [SerializeField] float startCircleScaleSize = 400;
    [SerializeField] float sizeReduceRate = 2f;
    [SerializeField] float minCircleRange = 50;
    [SerializeField] float shringSizePerShrink = 100;
    [SerializeField] GameObject shrinkTextObj = null;

    float baseZScale = 40;
    float currentSize;
    float targetShrinkSize;

    private void Start()
    {
        currentSize = startCircleScaleSize;
        targetShrinkSize = currentSize;

        transform.localScale = new Vector3(currentSize, currentSize, baseZScale);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameCircle());
        }

        shrinkTextObj.SetActive(false);
    }

    private void Update()
    {
        if (currentSize >= targetShrinkSize && currentSize > minCircleRange)
        {
            currentSize -= 20 * Time.deltaTime;
            shrinkTextObj.SetActive(true);
        }
        else
        {
            shrinkTextObj.SetActive(false);
        }

        transform.localScale = new Vector3(currentSize, currentSize, baseZScale);
    }

    IEnumerator StartGameCircle()
    {
        float timer = 0;

        while (currentSize > minCircleRange)
        {
            while (timer > 30)
            {
                photonView.RPC("RPCShrinkCircle", RpcTarget.AllBuffered);
                timer = 0;
                yield return null;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlanetHP>().isInCircle = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlanetHP>().isInCircle = true;
        }
    }

    [PunRPC]
    private void RPCShrinkCircle()
    {
        targetShrinkSize -= shringSizePerShrink;
    }
}