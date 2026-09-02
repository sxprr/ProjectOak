using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    // Reference to UI Parent
    private GameObject UserInterface;
    private Scrollbar DetectionUI;

    public TextMeshProUGUI itemText;
    

    // Start is called before the first frame update
    void Start()
    {
        DetectionUI = GetComponentInChildren<Scrollbar>();
        DetectionUI.size = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDetection(float detectionAmount)
    {
        detectionAmount = DetectionUI.size;

    }public void UpdateItem(int itemAmount)
    {
        itemText.text = ($"{itemAmount}/{itemAmount}");

    }
}
