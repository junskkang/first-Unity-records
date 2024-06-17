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
    public float speed = 1000.0f;

    FireCtrl fireCtrl;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3000.0f;

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        fireCtrl = GetComponent<FireCtrl>();

        //Destroy(gameObject, 4.0f);

        //������Ʈ Ǯ�� �ٽ� ȯ��
        StartCoroutine(PushObjectPool());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PushObjectPool(float time = 4.0f)
    {
        yield return new WaitForSeconds(time);

        //���� ���� �ʱ�ȭ
        transform.position = fireCtrl.firePos.transform.position;

        //�Ѿ� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
