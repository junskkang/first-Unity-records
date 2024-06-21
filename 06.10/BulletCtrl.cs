using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static MonsterCtrl;

public class BulletCtrl : MonoBehaviour
{
    //�Ѿ��� �ı���
    public int damage = 20;
    //�Ѿ� �߻� �ӵ�
    public float speed = 3000.0f;

    Vector3 firstPos = Vector3.zero;

    FireCtrl fireCtrl;

    GameObject firePos;

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

        Invoke("PushObjectPool", 4.0f);
        
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
}
