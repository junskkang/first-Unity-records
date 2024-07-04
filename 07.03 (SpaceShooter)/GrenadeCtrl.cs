using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCtrl : MonoBehaviour
{
    //����ȿ�� ��ƼŬ ���� ����
    public GameObject expEffect;

    //���� ���� Ÿ�̸�
    float timer = 2.0f;

    //�������� ������ �ؽ��� �迭
    public Texture[] textures;

    //����ź ���ư��� �ӵ�
    float speed = 600.0f;
    Vector3 forwardDir = Vector3.zero;

    bool isRot = true;
    // Start is called before the first frame update
    void Start()
    {
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];

        //���ư��� ���� ����
        transform.forward = forwardDir;
        transform.eulerAngles = new Vector3(20.0f, transform.eulerAngles.y,
                                           transform.eulerAngles.z);

        GetComponent<Rigidbody>().AddForce(forwardDir * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < timer)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                ExpGrenade();
            }
        }

        if (isRot)
        {
            transform.Rotate(new Vector3(Time.deltaTime * 190.0f, 0.0f, 0.0f), Space.Self);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        isRot = false;
    }
    void ExpGrenade()
    {
        //���� ȿ�� ��ƼŬ ����
        GameObject explosion = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

        //������ ������ �߽����� 10.0f �ݰ� ���� ���� �ִ� collider ��ü ����
        Collider[] colls = Physics.OverlapSphere(transform.position, 6.0f);    //��ġ�κ��� �ݰ����

        //������ �ݶ��̴� ��ü �� ���Ϳ� ���߷� ����
        MonsterCtrl monsterCtrl = null;
        foreach (Collider coll in colls) 
        {
            monsterCtrl = coll.GetComponent<MonsterCtrl>(); //������Ʈ���� �پ��ִ� �ֵ鸸 ã�ƿ�
            if (monsterCtrl == null)
                continue;

            monsterCtrl.TakeDamage(150);    //���Ϳ� ������ ��
        }

        Destroy(gameObject);
    }

    public void SetForwardDir(Vector3 dir)
    {
        forwardDir = new Vector3(dir.x, dir.y + 0.5f, dir.z);
    }
}
