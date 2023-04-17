using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{




    public void SetStatus(bool state)
    {
        gameObject.SetActive(state);
    }
    public bool GetStatus()
    {
        return gameObject.activeInHierarchy;
    }


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
