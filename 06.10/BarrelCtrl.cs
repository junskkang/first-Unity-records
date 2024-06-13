using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //���� ȿ�� ��ƼŬ ���� ����
    public GameObject explosionEffect;

    //�������� ������ �ؽ��� �迭
    public Texture[] textures;

    //�Ѿ� ���� Ƚ���� ������ų ����
    private int hitCount = 0;

    float massTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        //�ؽ��� ���� ��ü
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < massTimer)
        {
            massTimer -= Time.deltaTime;
            if (massTimer <= 0.0f)
            {
                Rigidbody rbody = this.GetComponent<Rigidbody>();
                if (rbody != null)
                    rbody.mass = 20.0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            //�浹�� �Ѿ� ����
            Destroy(collision.gameObject);

            //�Ѿ� ���� Ƚ���� ������Ű�� 3ȸ �̻��̸� ���� ó��
            if (++hitCount >= 3)
                ExpBarrel();
        }

        //if (collision.collider.tag == "Barrel")
        //{
        //    ExpBarrel();
        //}
    }

    public void ExpBarrel()
    {
        //���� ȿ�� ��ƼŬ ����
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration + 3.0f);

        //������ ������ �߽����� 10.0f �ݰ� ���� ���� �ִ� Collider ��ü ����
        Collider[] colls = Physics.OverlapSphere(transform.position, 5.0f);
        BarrelCtrl barrel = null;
        Rigidbody rbody = null;
        //������ Collider ��ü�� ���߷� ����
        foreach (Collider coll in colls)
        {
            barrel = coll.GetComponent<BarrelCtrl>();
            if (barrel == null) continue; 
            //barrel�� null�̶�� ���� �跲��Ʈ���� ���� ���� �ʴٴ� �ǹ� == �跲�� �ƴϴ�

            rbody = coll.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.mass = 1.0f;
                //���߷�, �߻���ġ, �ݰ�, ���� �ڱ�ġ�� ��
                rbody.AddExplosionForce(1000.0f, transform.position, 10.0f, 300, 0f);
                barrel.massTimer = 0.1f;

                //���������� ���� �κ�ũ�Լ�
                barrel.Invoke("ExpBarrel", 1.5f);
            }
        }

        //5�� �Ŀ� �巳�� ����
        Destroy(gameObject, 2.0f);
    }
}
