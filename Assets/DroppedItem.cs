using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DropItemType
{
    Gold,
    Point,
    Item,
}

public class DroppedItem : MonoBehaviour
{
    public DropItemType type;
    public int amount;
    public int itemId;

    private bool alreadyDone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyDone)
            return;

        if (other.CompareTag("Player"))
        {
            alreadyDone = true;
            switch (type)
            {
                case DropItemType.Gold:
                    StageManager.Instance.AddGold(amount);
                    break;

                case DropItemType.Point:
                    break;

                case DropItemType.Item:
                    break;
            }
            Destroy(transform.parent.gameObject);
        }
    }
}