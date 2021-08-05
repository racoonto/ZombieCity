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
    public enum GetMethodType
    {
        TriggerEnter,
        KeyDown,
    }

    public GetMethodType getMethod;
    public KeyCode keyCode = KeyCode.E;

    public DropItemType type;
    public int amount;
    public int itemId;

    private bool alreadyDone = false;

    private void Awake()
    {
        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyDone)
            return;

        if (other.CompareTag("Player"))
        {
            switch (getMethod)
            {
                case GetMethodType.TriggerEnter:
                    ItemAcquisition();
                    break;

                case GetMethodType.KeyDown:
                    enabled = true;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            enabled = false;
            ItemAcquisition();
        }
    }

    public Color color = Color.white;

    private void ItemAcquisition()
    {
        alreadyDone = true;
        switch (type)
        {
            case DropItemType.Gold:
                Actor.CreateTextEffect(amount, transform.position, color);
                StageManager.Instance.AddGold(amount);
                break;
        }

        transform.GetComponentInParent<MoveToPlayer>()?.StopCoroutine();
        Destroy(transform.root.gameObject);
    }
}