using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum BulletType
{
    BULLET,
    E_BULLET
}
public class BulletCtrl : MonoBehaviour
{
    //�Ѿ��� �ı���
    public int damage = 20;
    //�Ѿ� �߻� �ӵ�
    public float speed = 3000.0f;

    Vector3 firstPos = Vector3.zero;

    FireCtrl fireCtrl;

    GameObject firePos;

    //����ũ ����Ʈ ���� ����
    public GameObject sparkEffect;

    private void Awake()
    {
        speed = 3000.0f;

        fireCtrl = GameObject.FindObjectOfType<FireCtrl>();

        firePos = GameObject.Find("FirePos");
    }
    private void OnEnable()
    {

        //GetComponent<TrailRenderer>().time = 0.3f;

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //������Ʈ Ǯ�� �ٽ� ȯ��
        //StartCoroutine(PushObjectPool2());
    }

    void Start()
    {
        //Destroy(gameObject, 4.0f);

        if (gameObject.tag == "E_BULLET")
        {
            Destroy(gameObject, 4.0f);
            return;
        }
        else
        {
            Invoke("PushObjectPool", 4.0f);
        }       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public IEnumerator PushObjectPool2(float time = 4.0f)
    //{
    //    yield return new WaitForSeconds(time);

    //    GetComponent<TrailRenderer>().time = -1;
    //    //�Ѿ� ��Ȱ��ȭ
    //    gameObject.SetActive(false);


    //    yield break;

    //}

    public void PushObjectPool()
    {
        //���� ���� �ʱ�ȭ
        //GetComponent<TrailRenderer>().time = -1;
        //Ʈ���� ������ �ʱ�ȭ

        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);  //���������� �����ִ� �ӵ��� ����
        GetComponent<TrailRenderer>().Clear();

        //�Ѿ� ��Ȱ��ȭ
        gameObject.SetActive(false);

    }

    private void OnCollisionEnter(Collision coll)
    {

        //�Ʒ��� ���ܻ����� �� ��Ʈ�ѿ��� ������ �ϰ� �ֱ� ������ �ߺ��� ���ϱ� ���ؼ�
        if (coll.gameObject.name.Contains("Player") == true) return;

        if (coll.gameObject.name.Contains("Barrel") == true) return;

        if (coll.gameObject.name.Contains("Monster_") == true ) return;

        if (coll.gameObject.tag == "SideWall") return;

        if (coll.gameObject.tag == "BULLET") return;

        if (coll.gameObject.tag == "E_BULLET") return;

        
        //������� ���� �浹ü�� �ǹ���! ���� ��
        GameObject spark = Instantiate(sparkEffect, transform.position, Quaternion.identity);
        
        //ParticleSystem ������Ʈ�� ����ð�(duration)�� ���� �� ���� ó��
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        //�浹�� ���ӿ�����Ʈ ����
        //BulletCtrl bulletCtrl = coll.collider.GetComponent<BulletCtrl>();
        //�浹�� �Ѿ� ����
        PushObjectPool();
    }
}
