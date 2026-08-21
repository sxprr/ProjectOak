using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDebug : MonoBehaviour
{
    public PlayerPickUpDrop _controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void pConsoleMonitoring()
    {
        if(_controller.playerCamTransform.gameObject != null)
        {
            LogHandler.Log($"{_controller.playerCamTransform} I am testing the PickUp Debug class");
        }

    }
}
