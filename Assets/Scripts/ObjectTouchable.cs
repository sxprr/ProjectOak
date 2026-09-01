 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTouchable : MonoBehaviour
{
    public UnityEvent onVanish;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Vanish()
    {
        gameObject.SetActive(false);
        onVanish.Invoke();
    }
}
