using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenStar : MonoBehaviour
{
    public void SetStatus(bool state)
    {
        gameObject.SetActive(state);
    }
    public bool GetStatus()
    {
        return gameObject.activeInHierarchy;
    }
}
