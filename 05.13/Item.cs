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

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")    //바닥에 닿으면
        {
            rigid.isKinematic = true;   //더 이상 외부 물리효과를 받지 않음
            sphereCollider.enabled = false;
        }
    }
}
