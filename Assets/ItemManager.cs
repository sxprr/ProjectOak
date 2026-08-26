using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    // Lists can grow dynamically using .Add()
    public List<GameObject> itemCollect = new List<GameObject>();

    // Pass the target GameObject into the collection method

    // version that doesn't require a reference to the parent object. if whatever parent gameobject has
    // the script attached, just append it.

    private void Update()
    {
        
    }

    private void Start()
    {
        
    }

    // BUG: This method is subscribed to the 'onVanish' event, but nothing happens!
    public void ItemCollect(GameObject item)
    {
        // Check if the item touched has the required script
        if (item.TryGetComponent(out ObjectTouchable objectTouchable))
        {
            itemCollect.Add(objectTouchable.gameObject);

            // Log output with item count
            LogHandler.Log($"Added '{objectTouchable.name}' to inventory | Item count: {itemCollect.Count}");
        }
    }

}
