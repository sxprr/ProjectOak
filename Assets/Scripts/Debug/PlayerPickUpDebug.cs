using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDebug : MonoBehaviour
{
    // object reference to PlayerPickUpDrop
    public PlayerPickUpDrop _controller;

    [SerializeField] private float rayDistance;
    [SerializeField] private Color rayColor = Color.blue;
    [SerializeField] private Color secondRayColor;

    // Start is called before the first frame update
    void Start()
    {

        LogHandler.Log($"Green Ray is starting at {_controller.playerCamTransform.forward}");

        LogHandler.Log($"This is the blue ray, starting at {transform.rotation}");

    }

    // Update is called once per frame
    void Update()
    {
        //Green ray position
        Debug.DrawRay(_controller.playerCamTransform.position, _controller.playerCamTransform.forward * 4f, Color.green);

    }

    // Visual debug line: visible in Scene view (and Game view if 'Gizmos' is enabled)
    private void OnDrawGizmos()
    {
        if (_controller == null || _controller.playerCamTransform == null)
            return;

        

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_controller.playerCamTransform.position, _controller.playerCamTransform.forward * rayDistance);
    }

}
