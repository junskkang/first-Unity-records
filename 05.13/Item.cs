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
        if (collision.gameObject.tag == "Floor")    //�ٴڿ� ������
        {
            rigid.isKinematic = true;   //�� �̻� �ܺ� ����ȿ���� ���� ����
            sphereCollider.enabled = false;
        }
    }
}
