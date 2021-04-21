using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemID;
    public int _count;
    public string pickUpSound;

    public void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("item hit");
        if (Input.GetKey(KeyCode.Z))
        {
            //Debug.Log("get Z key");
            AudioManager.instance.Play(pickUpSound);
            Inventory.instance.GetAnItem(itemID, _count);
            Destroy(this.gameObject);
        }
    }
}
