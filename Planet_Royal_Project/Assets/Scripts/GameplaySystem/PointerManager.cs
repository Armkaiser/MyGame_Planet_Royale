using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    [SerializeField] private GameObject targetUi;
    [SerializeField] private GameObject pointerObj;

    public static PointerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestNewPlayerPointer(Transform targetForPointer, string ownerName)
    {
        ScreenTargetPointer newPointer = Instantiate(pointerObj, targetUi.transform).GetComponent<ScreenTargetPointer>();
        newPointer.SetTarget(targetForPointer, ownerName);
    }
}
