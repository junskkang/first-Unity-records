using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_Type       
{
    Ammo, Coin, Grenade, Heart, Weapon
}
public class Item : MonoBehaviour
{
    public Item_Type type;
    public int value;

    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
