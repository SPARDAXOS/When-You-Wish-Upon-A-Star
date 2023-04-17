using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private GameObject GridManagerObject = null;
    private GridManager GridManagerScript = null;

    public void RegisterGridManager(GameObject gridManager)
    {
        GridManagerObject = gridManager;
        GridManagerScript = GridManagerObject.GetComponent<GridManager>();
    }
    public void UpdateClickPos()
    {
        Vector3 WorldPos;
        Vector3 MousePos = Input.mousePosition;
        float PosZ = Mathf.Abs(transform.position.z);
        MousePos.z = PosZ;
        WorldPos = Camera.main.ScreenToWorldPoint(MousePos);
        GridManagerScript.SwitchCellTypeAt(WorldPos);
    }
    private void SetupLookAtTarget()
    {
        Vector3 LookAtTarget = GridManagerScript.GetCenterPoint();
        transform.position = new Vector3(LookAtTarget.x, LookAtTarget.y + 20.0f, LookAtTarget.z);
        transform.LookAt(LookAtTarget, Vector3.up);
    }
    private void Start()
    {
        SetupLookAtTarget();
    }
}
