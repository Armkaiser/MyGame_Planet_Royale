using System.Collections;
using System.Collections.Generic;
using NorapatUtility;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenTargetPointer : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] Transform pointerRect = null;
    [SerializeField] Transform nameTextRect = null;
    [SerializeField] Image graphic = null;
    [SerializeField] Sprite inSprite = null;

    Vector3 targetPos;
    Vector3 targetScreenPos;
    Vector3 camScreenPos;
    Vector3 offset;

    private void Start()
    {
        graphic.sprite = inSprite;
        offset = Vector3.up * 50;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        camScreenPos = Camera.main.transform.position;
        targetPos = target.position;

        camScreenPos.z = targetPos.z;

        Vector3 dir = (targetPos - camScreenPos).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x);
        angle = Mathf.Rad2Deg * angle - 90;

        float borderSize = Screen.width * 0.05f;
        bool isOffScreen = TransUtility.IsObjOffScreen(targetPos, borderSize);

        targetScreenPos = Camera.main.WorldToScreenPoint(targetPos);

        Vector3 cappedTargetScreenPos = Vector3.zero;
        cappedTargetScreenPos = targetScreenPos;

        if (isOffScreen)
        {
            nameTextRect.gameObject.SetActive(false);
            pointerRect.localEulerAngles = new Vector3(0, 0, angle);

            if (cappedTargetScreenPos.x <= borderSize)
            {
                cappedTargetScreenPos.x = borderSize;
            }
            if (cappedTargetScreenPos.x >= Screen.width - borderSize)
            {
                cappedTargetScreenPos.x = Screen.width - borderSize;
            }
            if (cappedTargetScreenPos.y <= borderSize)
            {
                cappedTargetScreenPos.y = borderSize;
            }
            if (cappedTargetScreenPos.y >= Screen.height - borderSize)
            {
                cappedTargetScreenPos.y = Screen.height - borderSize;
            }

            graphic.enabled = true;
            pointerRect.position = new Vector3(cappedTargetScreenPos.x, cappedTargetScreenPos.y, 0f);
        }
        else
        {
            graphic.enabled = false;
            pointerRect.position = new Vector3(cappedTargetScreenPos.x, cappedTargetScreenPos.y, 0f);
            pointerRect.localEulerAngles = new Vector3(0, 0, 0);

            nameTextRect.gameObject.SetActive(true);
            nameTextRect.position = targetScreenPos + offset;
        }
    }

    public void SetTarget(Transform newTarget, string playerName)
    {
        target = newTarget;
        nameTextRect.GetComponent<TextMeshProUGUI>().text = playerName;
    }
}