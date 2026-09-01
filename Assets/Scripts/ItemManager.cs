using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    // Lists can grow dynamically using .Add()
    public List<GameObject> itemCollect = new List<GameObject>();

    public UnityEvent onQoutaFull;
    public int requiredItems = 10;

    // Pass the target GameObject into the collection method

    // version that doesn't require a reference to the parent object. if whatever parent gameobject has
    // the script attached, just append it.

    private int itemNumber;

    private void Update()
    {
        
    }

    private void Start()
    {
        itemNumber = itemCollect.Count();
    }


    public void ItemCollect(GameObject item)
    {
        if (item.TryGetComponent(out ObjectTouchable objectTouchable))
        {
            // 1. Add the item first
            itemCollect.Add(objectTouchable.gameObject);

            // 2. Log output with accurate item count
            LogHandler.Log($"Added '{objectTouchable.name}' to inventory | Item count: {itemCollect.Count}");

            // 3. Check the count AFTER adding
            if (itemCollect.Count == requiredItems)
            {
                onQoutaFull.Invoke();
            }
        }
    }

}
